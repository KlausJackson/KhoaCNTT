using KhoaCNTT.API.Models.Comment;
using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.News;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.Services;

public class NewsService(
    INewsRepository newsRepo,
    INewsResourceRepository newsResourceRepo,
    INewsRequestRepository newsRequestRepo,
    INewsApprovalRepository newsApprovalRepo,
    ICommentRepository commentRepo) : INewsService
{
    private static readonly Dictionary<string, DateTime> _viewCooldown = [];

    // ── News ─────────────────────────────────────────────────────

    public async Task<IEnumerable<NewsResponse>> GetAllNewsAsync()
    {
        var news = await newsRepo.GetAllWithResourceAsync();
        return news.Select(MapToResponse);
    }

    public async Task<NewsResponse> GetNewsByIdAsync(int id)
    {
        var news = await newsRepo.GetByIdWithResourceAsync(id)
            ?? throw new NotFoundException(nameof(News), id);

        var cooldownKey = $"view_{id}";
        var now = DateTime.UtcNow;
        if (!_viewCooldown.TryGetValue(cooldownKey, out var lastView) || (now - lastView).TotalSeconds > 5)
        {
            _viewCooldown[cooldownKey] = now;
            await newsRepo.IncrementViewCountAsync(id);
            news.ViewCount++;
        }

        return MapToResponse(news);
    }

    // ── Requests ─────────────────────────────────────────────────

    public async Task<IEnumerable<NewsRequestResponse>> GetPendingRequestsAsync()
    {
        var requests = await newsRequestRepo.GetPendingAsync();
        return requests.Select(MapRequestToResponse);
    }

    public async Task<NewsRequestResponse> SubmitCreateRequestAsync(
        CreateNewsRequest dto, int submitterId, bool isSenior)
    {
        var resource = new NewsResource
        {
            Content = dto.Content,
            CreatedBy = submitterId,
            CreatedAt = DateTime.UtcNow
        };
        await newsResourceRepo.AddAsync(resource);

        var request = new NewsRequest
        {
            TargetNewsId = null,
            NewResourceId = resource.Id,
            OldResourceId = null,
            Title = dto.Title,
            NewsType = dto.NewsType,
            RequestType = RequestType.CreateNew,
            IsProcessed = false,
            CreatedAt = DateTime.UtcNow
        };
        await newsRequestRepo.AddAsync(request);

        if (isSenior)
            await ApproveRequestInternalAsync(submitterId, request.Id, ApprovalDecision.Approved, null);

        return MapRequestToResponse(request);
    }

    public async Task<NewsRequestResponse> SubmitReplaceRequestAsync(
        UpdateNewsRequest dto, int submitterId, bool isSenior)
    {
        var existingNews = await newsRepo.GetByIdWithResourceAsync(dto.TargetNewsID)
            ?? throw new NotFoundException(nameof(News), dto.TargetNewsID);

        var newResource = new NewsResource
        {
            Content = dto.Content,
            CreatedBy = submitterId,
            CreatedAt = DateTime.UtcNow
        };
        await newsResourceRepo.AddAsync(newResource);

        var request = new NewsRequest
        {
            TargetNewsId = dto.TargetNewsID,
            NewResourceId = newResource.Id,
            OldResourceId = existingNews.CurrentResourceId,
            Title = dto.Title,
            NewsType = dto.NewsType,
            RequestType = RequestType.Replace,
            IsProcessed = false,
            CreatedAt = DateTime.UtcNow
        };
        await newsRequestRepo.AddAsync(request);

        if (isSenior)
            await ApproveRequestInternalAsync(submitterId, request.Id, ApprovalDecision.Approved, null);

        return MapRequestToResponse(request);
    }

    public async Task DeleteNewsAsync(int newsId)
    {
        var news = await newsRepo.GetByIdWithResourceAsync(newsId)
            ?? throw new NotFoundException(nameof(News), newsId);

        await newsRepo.DeleteAsync(news);
    }

    public async Task<NewsApprovalResponse> ProcessApprovalAsync(
        ApproveNewsRequest dto, int approverId) =>
        await ApproveRequestInternalAsync(approverId, dto.NewsRequestID, dto.Decision, dto.RejectReason);

    // ── Comments ─────────────────────────────────────────────────

    public async Task<IEnumerable<CommentResponse>> GetCommentsByNewsIdAsync(int newsId)
    {
        var comments = await commentRepo.GetByNewsIdAsync(newsId);
        return comments.Select(MapCommentToResponse);
    }

    public async Task<CommentResponse> AddCommentAsync(
        int newsId, CreateCommentRequest dto, string msv, string studentName)
    {
        _ = await newsRepo.GetByIdWithResourceAsync(newsId)
            ?? throw new NotFoundException(nameof(News), newsId);

        var comment = new Comment
        {
            NewsId = newsId,
            MSV = msv,
            StudentName = studentName,
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await commentRepo.AddAsync(comment);
        return MapCommentToResponse(comment);
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await commentRepo.GetByIdAsync(commentId)
            ?? throw new NotFoundException(nameof(Comment), commentId);

        await commentRepo.DeleteAsync(comment);
    }

    // ── Private helpers ──────────────────────────────────────────

    private async Task<NewsApprovalResponse> ApproveRequestInternalAsync(
        int approverId, int requestId, ApprovalDecision decision, string? rejectReason)
    {
        var newsRequest = await newsRequestRepo.GetByIdWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(NewsRequest), requestId);

        if (newsRequest.IsProcessed)
            throw new BusinessRuleException("Yêu cầu này đã được xử lý.");

        var approval = new NewsApproval
        {
            ApproverId = approverId,
            NewsRequestId = requestId,
            Decision = decision,
            Reason = rejectReason,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await newsApprovalRepo.AddAsync(approval);

        if (decision == ApprovalDecision.Approved)
        {
            if (newsRequest.RequestType == RequestType.CreateNew)
            {
                var news = new News
                {
                    Title = newsRequest.Title,
                    CurrentResourceId = newsRequest.NewResourceId,
                    ViewCount = 0,
                    NewsType = newsRequest.NewsType,
                    CreatedBy = approverId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await newsRepo.AddAsync(news);
            }
            else
            {
                var targetNews = await newsRepo.GetByIdWithResourceAsync(newsRequest.TargetNewsId!.Value)
                    ?? throw new NotFoundException(nameof(News), newsRequest.TargetNewsId);

                targetNews.Title = newsRequest.Title;
                targetNews.NewsType = newsRequest.NewsType;
                targetNews.CurrentResourceId = newsRequest.NewResourceId;
                targetNews.UpdatedAt = DateTime.UtcNow;
                await newsRepo.UpdateAsync(targetNews);
            }
        }

        newsRequest.IsProcessed = true;
        await newsRequestRepo.UpdateAsync(newsRequest);

        return new NewsApprovalResponse
        {
            NewsApprovalID = approval.Id,
            ApproverID = approval.ApproverId,
            NewsRequestID = approval.NewsRequestId,
            Decision = approval.Decision,
            RejectReason = approval.Reason,
            CreatedAt = approval.CreatedAt
        };
    }

    // ── Mappers ───────────────────────────────────────────────────

    private static NewsResponse MapToResponse(News n) => new()
    {
        NewsID = n.Id,
        Title = n.Title,
        Content = n.CurrentResource?.Content ?? string.Empty,
        ViewCount = n.ViewCount,
        NewsType = n.NewsType,
        CreatedBy = n.CreatedBy,
        CreatedAt = n.CreatedAt,
        UpdatedAt = n.UpdatedAt
    };

    private static NewsRequestResponse MapRequestToResponse(NewsRequest r) => new()
    {
        NewsRequestID = r.Id,
        TargetNewsID = r.TargetNewsId,
        Title = r.Title,
        NewsType = r.NewsType,
        RequestType = r.RequestType,
        IsProcessed = r.IsProcessed,
        CreatedAt = r.CreatedAt
    };

    private static CommentResponse MapCommentToResponse(Comment c) => new()
    {
        CommentID = c.Id,
        NewsId = c.NewsId,
        MSV = c.MSV,
        StudentName = c.StudentName,
        Content = c.Content,
        CreatedAt = c.CreatedAt
    };
}