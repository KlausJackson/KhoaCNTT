
using AutoMapper;
using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Entities.FileEntities;
using KhoaCNTT.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;


namespace KhoaCNTT.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        private readonly IFileRepository _fileRepo; // Quản lý Metadata (Hiển thị)
        private readonly IFileRequestRepository _requestRepo;    // Quản lý Yêu cầu duyệt
        private readonly IFileResourceRepository _resourceRepo;  // Quản lý File vật lý
        private readonly IFileApprovalRepository _approvalRepo;  // Quản lý Lịch sử duyệt

        private readonly IFileStorageService _storage; // Lưu file vật lý
        private readonly IMapper _mapper;
        private readonly ISubjectRepository _subjectRepo; // validate môn
        private readonly IAdminRepository _adminRepo;

        public FileService(
            IFileRepository fileRepo,
            IFileRequestRepository requestRepo,
            IFileResourceRepository resourceRepo,
            IFileApprovalRepository approvalRepo,
            IFileStorageService storage,
            IMapper mapper,
            ISubjectRepository subjectRepo,
            IAdminRepository adminRepo,
            IWebHostEnvironment env)
        {
            _env = env;
            _fileRepo = fileRepo;
            _requestRepo = requestRepo;
            _resourceRepo = resourceRepo;
            _approvalRepo = approvalRepo;
            _storage = storage;
            _mapper = mapper;
            _subjectRepo = subjectRepo;
            _adminRepo = adminRepo;
        }

        // UPLOAD & DUYỆT

        public async Task ActionFileAsync(UploadFileRequest request, string username, RequestType type, int? targetFileId = null)
        {
            // Validate
            await CheckSubjectCode(request.SubjectCode);
            CheckTitle(request.Title);
            var admin = await _adminRepo.GetByUsernameAsync(username);
            if (admin == null) throw new BusinessRuleException("Không tìm thấy thông tin Admin.");
            if (request.File == null) throw new BusinessRuleException("Vui lòng tải lên tài liệu.");

            // Lưu file vật lý
            var path = await _storage.SaveFileAsync(request.File);

            // Tạo Resource
            var resource = new FileResource
            {
                FileName = request.File.FileName,
                FilePath = path,
                CreatedBy = admin.Id,
                Size = request.File.Length,
                CreatedAt = DateTime.UtcNow
            };
            await _resourceRepo.AddAsync(resource);

            // Lay Id oldResource
            int? oldResourceId = null;
            if (type == RequestType.Replace && targetFileId != null)
            {
                var targetFile = await _fileRepo.GetByIdAsync(targetFileId.Value) ?? throw new BusinessRuleException("Không tồn tại tài liệu này.");
                oldResourceId = targetFile.CurrentResourceId;
            }

            // Tạo Request
            var fileRequest = new FileRequest
            {
                RequestType = type,
                FileType = request.FileType,
                Title = request.Title,
                SubjectCode = request.SubjectCode,
                Permission = request.Permission,
                NewResourceId = resource.Id,
                OldResourceId = oldResourceId,
                TargetFileId = targetFileId,
                IsProcessed = false,
                CreatedAt = DateTime.UtcNow
            };
            await _requestRepo.AddAsync(fileRequest);

            // Auto Approve nếu là Admin Cấp 1, 2
            if (admin.Level <= 2)
            {
                await ApproveFileAsync(fileRequest.Id, true, "Auto Approved", username);
            }
        }

        public async Task UploadFileAsync(UploadFileRequest request, string username)
        {
            await ActionFileAsync(request, username, RequestType.CreateNew, null);
        }

        public async Task ReplaceFileAsync(int targetFileId, UploadFileRequest request, string username)
        {
            await ActionFileAsync(request, username, RequestType.Replace, targetFileId);
        }

        public async Task ApproveFileAsync(int requestId, bool isApproved, string? reason, string username)
        {
            // Validate
            var admin = await _adminRepo.GetByUsernameAsync(username);
            if (admin == null) throw new BusinessRuleException("Không tìm thấy thông tin Admin.");

            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) throw new NotFoundException("Request", requestId);
            if (request.IsProcessed) throw new BusinessRuleException("Yêu cầu đã được xử lý.");

            var decision = isApproved ? ApprovalDecision.Approved : ApprovalDecision.Rejected;

            // Lưu lịch sử duyệt
            var approval = new FileApproval
            {
                FileRequestId = requestId,
                ApproverId = admin.Id,
                Decision = decision,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await _approvalRepo.AddAsync(approval);

            if (isApproved)
            { // Nếu chấp nhận
                // tạo mới thì tạo FileEntity mới
                if (request.RequestType == RequestType.CreateNew)
                {
                    var newFile = new FileEntity
                    {
                        Title = request.Title,
                        SubjectCode = request.SubjectCode,
                        Permission = request.Permission,
                        CurrentResourceId = request.NewResourceId,
                        FileType = request.FileType,
                        ViewCount = 0,
                        DownloadCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = admin.Id
                    };
                    await _fileRepo.AddAsync(newFile);
                }
                else
                { // thay thế thì cập nhật FileEntity cũ
                    var targetFile = await _fileRepo.GetByIdAsync(request.TargetFileId.Value);
                    targetFile.CurrentResourceId = request.NewResourceId; // Trỏ sang file vật lý mới
                    targetFile.UpdatedAt = DateTime.UtcNow;
                    await _fileRepo.UpdateAsync(targetFile);
                }
            }

            request.IsProcessed = true;
            request.UpdatedAt = DateTime.UtcNow;
            await _requestRepo.UpdateAsync(request);
        }

        public async Task<PagedResult<FileRequestDto>> GetPendingRequestsAsync()
        {
            var requests = await _requestRepo.GetPendingRequestsWithDetailsAsync();
            return new PagedResult<FileRequestDto>
            {
                Total = requests.Count,
                Items = _mapper.Map<List<FileRequestDto>>(requests)
            };
        }

        // SEARCH, GET, DOWNLOAD

        public async Task<PagedResult<FileDto>> SearchFilesAsync(string? keyword, List<string>? subjectCodes, string? fileType, int page, int pageSize, string userId, bool isAdmin)
        {
            var result = await _fileRepo.SearchAsync(keyword, subjectCodes, fileType, page, pageSize);

            int count = result.Total;
            var sendToClient = new List<FileDto>();

            foreach (var entity in result.Items)
            {
                // Admin thấy tất cả
                if (isAdmin)
                {
                    sendToClient.Add(_mapper.Map<FileDto>(entity));
                    continue;
                }

                // Hidden -> chỉ admin
                if (entity.Permission == FilePermission.Hidden)
                {
                    continue;
                }
                sendToClient.Add(_mapper.Map<FileDto>(entity));
            }

            return new PagedResult<FileDto>
            {
                Total = count - pageSize + sendToClient.Count,
                Items = sendToClient,
            };
        }

        public async Task<(Stream stream, string contentType)> GetFileByIdAsync(int id, string? userId, bool isAdmin)
        {
            var file = await _fileRepo.GetByIdAsync(id) ?? throw new BusinessRuleException("Không tồn tại tài liệu này."); // Lấy FileEntity

            // CHECK QUYỀN
            CheckPermission(file.Permission, userId, isAdmin, "xem");

            var extension = Path.GetExtension(file.CurrentResource.FileName).ToLower();

            // chỉ hỗ trợ xem trước PDF
            if (extension != ".pdf")
            {
                throw new BusinessRuleException("Tài liệu này không hỗ trợ xem trước.");
            }

            var stream = _storage.GetFileStream(file.CurrentResource.FilePath) ?? throw new NotFoundException("File vật lý không tồn tại", id);
            using var originalPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

            var previewPdf = new PdfDocument();

            if (originalPdf.PageCount >= 1)
                previewPdf.AddPage(originalPdf.Pages[0]);

            if (originalPdf.PageCount >= 2)
                previewPdf.AddPage(originalPdf.Pages[1]);

            var memoryStream = new MemoryStream();
            previewPdf.Save(memoryStream, false);
            memoryStream.Position = 0;

            // Tăng View
            file.ViewCount++;
            file.UpdatedAt = DateTime.UtcNow;
            await _fileRepo.UpdateAsync(file);

            return (memoryStream, "application/pdf");
        }

        public async Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(int fileId, string? userId, bool isAdmin)
        {
            // Lấy FileEntity
            var file = await _fileRepo.GetByIdAsync(fileId) ?? throw new BusinessRuleException("Không tồn tại tài liệu này.");

            // Check Quyền
            CheckPermission(file.Permission, userId, isAdmin, "tải về");

            // Lấy Resource vật lý
            // FileEntity giữ ID của Resource đang dùng
            var resource = await _resourceRepo.GetByIdAsync(file.CurrentResourceId) ?? throw new NotFoundException("File Resource", file.CurrentResourceId);

            // Tăng lượt tải
            file.DownloadCount++;
            await _fileRepo.UpdateAsync(file);

            string fileName = resource.FileName;

            // Stream file từ ổ cứng
            var stream = _storage.GetFileStream(resource.FilePath);
            return stream == null ? throw new NotFoundException("File vật lý", fileName) : ((Stream stream, string contentType, string fileName))(stream, GetContentType(fileName), fileName);
        }

        public async Task DeleteFileAsync(int fileId)
        {
            // Xóa FileEntity
            var file = await _fileRepo.GetByIdAsync(fileId) ?? throw new BusinessRuleException("Không tồn tại tài liệu này.");
            await _fileRepo.DeleteAsync(file);
        }

        public async Task UpdateFileInfoAsync(int fileId, UpdateFileRequest request)
        {
            // Chỉ sửa metadata (Title, Subject...), không sửa file vật lý

            var file = await _fileRepo.GetByIdAsync(fileId) ?? throw new BusinessRuleException("Không tồn tại tài liệu này.");
            await CheckSubjectCode(request.SubjectCode);
            CheckTitle(request.Title);

            file.Title = request.Title;
            file.SubjectCode = string.IsNullOrWhiteSpace(request.SubjectCode)
                ? null
                : request.SubjectCode;
            file.Permission = request.Permission;
            file.FileType = request.FileType;
            await _fileRepo.UpdateAsync(file);
        }

        public async Task<Dictionary<string, int>> GetStatsByFileTypeAsync() => await _fileRepo.GetStatsByFileTypeAsync();
        public async Task<Dictionary<string, int>> GetStatsBySubjectAsync() => await _fileRepo.GetStatsBySubjectAsync();
        public async Task<Dictionary<string, int>> GetStatsByTrafficAsync() => await _fileRepo.GetStatsByTrafficAsync();


        // Hàm phụ check quyền
        private static void CheckPermission(FilePermission permission, string? userId, bool isAdmin, string action)
        {
            if (isAdmin) return;
            switch (permission)
            {
                case FilePermission.Hidden:
                    throw new BusinessRuleException("Tài liệu bị ẩn.");

                case FilePermission.StudentDownload:
                    if (string.IsNullOrEmpty(userId)) throw new BusinessRuleException($"Cần đăng nhập để {action} tài liệu này.");
                    break;

                case FilePermission.PublicRead or FilePermission.StudentRead: 
                    if (action == "tải về") throw new BusinessRuleException("Không được tải tài liệu này.");
                     break;
                    
                case FilePermission.PublicDownload: break;
            }
        }

        private static void CheckTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleException("Tiêu đề không được để trống.");
            if (title.Length < 3 || title.Length > 20)
            {
                throw new BusinessRuleException("Tiêu đề phải có độ dài từ 3-20 ký tự.");
            }
            return;
        }


        private async Task CheckSubjectCode(string? subjectCode)
        {
            if (string.IsNullOrWhiteSpace(subjectCode))
            {
                return;
            }
            var subject = await _subjectRepo.GetByCodeAsync(subjectCode);
            if (subject == null) throw new BusinessRuleException("Mã môn học không tồn tại");
        }


        public static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".csv" => "text/csv",
                ".xml" => "application/xml",
                ".json" => "application/json",
                ".tar" => "application/x-tar",
                ".gz" => "application/gzip",
                ".mp4" => "video/mp4",
                ".mp3" => "audio/mpeg",
                ".avi" => "video/x-msvideo",
                ".mkv" => "video/x-matroska",
                ".exe" => "application/vnd.microsoft.portable-executable",
                ".dll" => "application/vnd.microsoft.portable-executable",
                ".iso" => "application/x-iso9660-image",
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".py" => "text/x-python",
                ".java" => "text/x-java-source",
                ".cpp" => "text/x-c++src",
                ".cs" => "text/x-csharp",
                ".go" => "text/x-go",
                ".rb" => "text/x-ruby",
                ".php" => "text/x-php",
                ".swift" => "text/x-swift",
                ".kt" => "text/x-kotlin",
                ".rs" => "text/x-rust",
                ".md" => "text/markdown",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"

            };
            return contentType;
        }
    }
}


