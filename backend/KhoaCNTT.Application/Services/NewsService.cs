using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Application.Interfaces.Services.INewsServices;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Domain.Enums;
// BÍ DANH
using NewsEntity = KhoaCNTT.Domain.Entities.NewsEntities.News;

namespace KhoaCNTT.Application.Services
{
    public class NewsService(
        INewsRepository newsRepo,
        INewsRequestRepository requestRepo,
        INewsApprovalRepository approvalRepo,
        INewsResourceRepository resourceRepo,
        IAdminRepository adminRepo,
        ICommentRepository commentRepo) : INewsService
    {
        // ─── VALIDATION ───
        private static void CheckTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleException("Tiêu đề không được để trống.");
            if (title.Length < 3 || title.Length > 100) throw new BusinessRuleException("Tiêu đề phải từ 3-100 ký tự.");
        }

        private static void CheckContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new BusinessRuleException("Nội dung không được để trống.");
            if (content.Length < 10) throw new BusinessRuleException("Nội dung quá ngắn.");
        }

        // ─── ACTION CHUNG CHO CREATE & UPDATE ───
        // Đã sửa tham số thứ 3 nhận trực tiếp Enum NewsType (CS1503)
        private async Task ActionNewsRequestAsync(string title, string content, NewsType newsType, string username, RequestType reqType, int? targetNewsId = null)
        {
            CheckTitle(title);
            CheckContent(content);

            var admin = await adminRepo.GetByUsernameAsync(username)
                ?? throw new BusinessRuleException("Không tìm thấy thông tin Admin");

            // 1. Lưu Resource vật lý cho News
            var resource = new NewsResource
            {
                Content = content,
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow
            };
            await resourceRepo.AddAsync(resource);

            // 2. Lấy Resource cũ nếu là Update
            int? oldResourceId = null;
            if (reqType == RequestType.Replace && targetNewsId.HasValue)
            {
                var targetNews = await newsRepo.GetByIdAsync(targetNewsId.Value)
                    ?? throw new NotFoundException("News", targetNewsId.Value);
                oldResourceId = targetNews.CurrentResourceId;
            }

            // 3. Tạo Request chờ duyệt
            var request = new NewsRequest
            {
                Title = title,
                NewsType = newsType, // Nhận Enum trực tiếp
                RequestType = reqType,
                NewResourceId = resource.Id,
                OldResourceId = oldResourceId,
                TargetNewsId = targetNewsId,
                IsProcessed = false,
                CreatedAt = DateTime.UtcNow
            };
            await requestRepo.AddAsync(request);

            // 4. Tự động duyệt nếu là Admin Level 1, 2
            if (admin.Level <= 2)
            {
                await ApproveNewsAsync(request.Id, true, "Auto Approved by System", username);
            }
        }

        public async Task CreateNewsAsync(CreateNewsRequest req, string username) =>
            await ActionNewsRequestAsync(req.Title, req.Content, req.NewsType, username, RequestType.CreateNew);

        public async Task UpdateNewsAsync(int id, UpdateNewsRequest req, string username) =>
            await ActionNewsRequestAsync(req.Title, req.Content, req.NewsType, username, RequestType.Replace, id);

        public async Task ApproveNewsAsync(int requestId, bool isApproved, string? reason, string username)
        {
            var admin = await adminRepo.GetByUsernameAsync(username) ?? throw new BusinessRuleException("Không tìm thấy Admin duyệt");
            var req = await requestRepo.GetByIdAsync(requestId) ?? throw new NotFoundException("NewsRequest", requestId);

            if (req.IsProcessed) throw new BusinessRuleException("Yêu cầu đã được xử lý");

            var approval = new NewsApproval
            {
                NewsRequestId = requestId,
                ApproverId = admin.Id,
                Decision = isApproved ? ApprovalDecision.Approved : ApprovalDecision.Rejected,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await approvalRepo.AddAsync(approval);

            if (isApproved)
            {
                var newResource = await resourceRepo.GetByIdAsync(req.NewResourceId);
                int creatorAdminId = newResource?.CreatedBy ?? admin.Id;

                if (req.RequestType == RequestType.CreateNew)
                {
                    await newsRepo.AddAsync(new NewsEntity
                    {
                        Title = req.Title,
                        NewsType = req.NewsType,
                        CurrentResourceId = req.NewResourceId,
                        ViewCount = 0,
                        CreatedBy = creatorAdminId, // CS0117: Sửa thành CreatedBy theo Entity
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else if (req.RequestType == RequestType.Replace && req.TargetNewsId.HasValue)
                {
                    var news = await newsRepo.GetByIdAsync(req.TargetNewsId.Value);
                    if (news != null)
                    {
                        news.Title = req.Title;
                        news.NewsType = req.NewsType;
                        news.CurrentResourceId = req.NewResourceId;
                        news.UpdatedAt = DateTime.UtcNow;
                        await newsRepo.UpdateAsync(news);
                    }
                }
            }

            req.IsProcessed = true;
            req.UpdatedAt = DateTime.UtcNow;
            await requestRepo.UpdateAsync(req);
        }

        public async Task<NewsResponse> GetNewsByIdAsync(int id)
        {
            var news = await newsRepo.GetByIdWithDetailsAsync(id) ?? throw new NotFoundException("News", id);

            await newsRepo.IncrementViewCountAsync(id);

            return new NewsResponse
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.CurrentResource?.Content ?? "",
                CreatedAt = news.CreatedAt,
                UpdatedAt = news.UpdatedAt,
                CreatedBy = news.Admin?.FullName ?? "Unknown", // CS0023: Chấm từ Navigation Property 'Admin'
                ViewCount = news.ViewCount + 1,
                Comments = news.Comments?.Select(c => new CommentResponse
                {
                    CommentID = c.Id,
                    Content = c.Content,
                    StudentName = c.StudentName,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? new List<CommentResponse>()
            };
        }

        public async Task<PagedResult<NewsResponse>> SearchNewsAsync(string? keyword, string? newsType, int page, int pageSize, string? userId, bool isAdmin)
        {
            var data = await newsRepo.SearchAsync(keyword, newsType, page, pageSize);
            return new PagedResult<NewsResponse>
            {
                Total = data.Total,
                Items = data.Items.Select(n => new NewsResponse
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.CurrentResource?.Content ?? "",
                    CreatedAt = n.CreatedAt,
                    CreatedBy = n.Admin?.FullName ?? "Unknown", // CS0023: Chấm từ Navigation Property 'Admin'
                    ViewCount = n.ViewCount
                }).ToList()
            };
        }

        public async Task<PagedResult<NewsRequestDto>> GetPendingRequestsAsync()
        {
            var reqs = (await requestRepo.GetPendingRequestsAsync()).ToList();

            return new PagedResult<NewsRequestDto>
            {
                Total = reqs.Count,
                Items = reqs.Select(r => new NewsRequestDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    RequestType = r.RequestType.ToString(),
                    RequesterName = r.NewResource?.Admin?.FullName ?? "Unknown",
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }

        public async Task DeleteNewsAsync(int id)
        {
            var news = await newsRepo.GetByIdAsync(id) ?? throw new NotFoundException("News", id);
            await newsRepo.DeleteAsync(news);
        }

        public async Task AddCommentAsync(int newsId, string msv, string studentName, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new BusinessRuleException("Nội dung không hợp lệ");
            var comment = new Comment { NewsId = newsId, MSV = msv, StudentName = studentName, Content = content, CreatedAt = DateTime.UtcNow };
            await commentRepo.AddAsync(comment);
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await commentRepo.GetByIdAsync(id) ?? throw new NotFoundException("Comment", id);
            await commentRepo.DeleteAsync(comment);
        }

        public async Task<Dictionary<string, int>> GetStatsByTypeAsync() => await newsRepo.GetStatsByTypeAsync();
        public async Task<Dictionary<string, int>> GetStatsByMonthAsync(int year) => await newsRepo.GetStatsByMonthAsync(year);
    }
}