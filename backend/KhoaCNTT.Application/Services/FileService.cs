
using AutoMapper;
using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _repo;
        private readonly IFileStorageService _storage;
        private readonly IMapper _mapper;
        private readonly ISubjectRepository _subjectRepo;

        public FileService(IFileRepository repo, IFileStorageService storage, IMapper mapper, ISubjectRepository subjectRepo)
        {
            _repo = repo;
            _storage = storage;
            _mapper = mapper;
            _subjectRepo = subjectRepo;
        }

        public async Task<List<FileResourceDto>> GetPendingFilesAsync()
        {
            var entities = await _repo.GetPendingFilesAsync();
            return _mapper.Map<List<FileResourceDto>>(entities);
        }

        public async Task UploadFileAsync(UploadFileRequest request, int adminLevel, string username)
        {
            // Validate kích thước file
            const long MaxSize = 200 * 1024 * 1024;
            if (request.File.Length > MaxSize)
            {
                throw new BusinessRuleException("File quá lớn. Tối đa 200MB.");
            }

            // Validate Subject
            var subject = await _subjectRepo.GetByCodeAsync(request.SubjectCode);

            if (subject == null)
            {
                throw new BusinessRuleException($"Mã môn học '{request.SubjectCode}' không tồn tại trong hệ thống.");
            }

            // Tự động điền tên môn học chuẩn từ DB
            request.SubjectName = subject.SubjectName;

            // 1. Lưu file vật lý
            var filePath = await _storage.SaveFileAsync(request.File);

            // 2. Map từ Request sang Entity
            var fileEntity = new FileResource
            {
                Title = request.Title,
                FileName = request.File.FileName,
                FilePath = filePath,
                ContentType = request.File.ContentType,
                Size = request.File.Length,
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Permission = request.Permission,

                // 3. Thông tin Admin
                CreatedByUsername = username,
                CreatedAt = DateTime.Now
            };

            // 4. Logic Phân quyền
            if (adminLevel == 1 || adminLevel == 2)
            {
                fileEntity.Status = FileStatus.Approved;
                fileEntity.ApprovedByUsername = username;
            }
            else
            {
                fileEntity.Status = FileStatus.Pending;
            }

            await _repo.AddAsync(fileEntity);
        }

        public async Task ApproveFileAsync(int fileId, bool isApproved, string? rejectReason, string approverName)
        {
            var file = await _repo.GetByIdAsync(fileId);
            if (file == null) throw new NotFoundException("File", fileId);

            if (isApproved)
            {
                file.Status = FileStatus.Approved;
                file.ApprovedByUsername = approverName;
                file.RejectReason = null;

                // Xử lý thay thế file cũ (nếu có)
                if (file.ParentFileId.HasValue)
                {
                    var oldFile = await _repo.GetByIdAsync(file.ParentFileId.Value);
                    if (oldFile != null)
                    {
                        // Logic: Ẩn file cũ hoặc đánh dấu đã bị thay thế
                        // Ví dụ: Ta có thể thêm trạng thái 'Archived' vào Enum FileStatus
                        // oldFile.Status = FileStatus.Archived;
                        // await _repo.UpdateAsync(oldFile);
                    }
                }
            }
            else
            {
                file.Status = FileStatus.Rejected;
                file.RejectReason = rejectReason;
                file.ApprovedByUsername = approverName;
            }

            await _repo.UpdateAsync(file);
        }


        public async Task UpdateFileInfoAsync(int fileId, UpdateFileRequest request)
        {
            var file = await _repo.GetByIdAsync(fileId);
            if (file == null) throw new NotFoundException("File", fileId);

            // Cập nhật thông tin
            file.Title = request.Title;
            file.SubjectCode = request.SubjectCode;
            file.SubjectName = request.SubjectName;
            file.Permission = request.Permission;

            await _repo.UpdateAsync(file);
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = await _repo.GetByIdAsync(fileId);
            if (file == null) throw new NotFoundException("File", fileId);

            // 1. Xóa file vật lý (trên ổ cứng)
            await _storage.DeleteFileAsync(file.FilePath);

            // 2. Xóa trong DB
            await _repo.DeleteAsync(file);
        }

        public async Task<FileResourceDto> GetFileByIdAsync(int id, string? userId, bool isAdmin)
        {
            var file = await _repo.GetByIdAsync(id);
            if (file == null) throw new NotFoundException("File", id);

            // Check quyền xem
            if (!isAdmin)
            {
                if (file.Permission == FilePermission.Hidden)
                    throw new BusinessRuleException("Tài liệu không tồn tại hoặc bị ẩn.");

                // Các quyền Student Read/Download thì phải đăng nhập mới thấy Info
                if ((file.Permission == FilePermission.StudentRead || file.Permission == FilePermission.StudentDownload)
                    && string.IsNullOrEmpty(userId))
                {
                    throw new BusinessRuleException("Vui lòng đăng nhập để xem tài liệu này.");
                }
            }

            file.ViewCount++;
            await _repo.UpdateAsync(file);

            return _mapper.Map<FileResourceDto>(file);
        }

        public async Task<(Stream, string, string)> DownloadFileAsync(int fileId, string? userId, bool isAdmin)
        {
            var file = await _repo.GetByIdAsync(fileId);
            if (file == null) throw new NotFoundException("File", fileId);

            // 1. CHECK QUYỀN
            if (!isAdmin)
            {
                switch (file.Permission)
                {
                    case FilePermission.Hidden:
                        throw new BusinessRuleException("File này đang bị ẩn.");

                    case FilePermission.PublicRead:
                        // Khách xem được, không tải được
                        throw new BusinessRuleException("Tài liệu này chỉ cho phép xem, không cho phép tải.");

                    case FilePermission.StudentRead:
                        // Sinh viên xem được, không tải được
                        throw new BusinessRuleException("Tài liệu này chỉ cho phép sinh viên xem, không cho phép tải.");

                    case FilePermission.StudentDownload:
                        // Sinh viên tải được -> Cần đăng nhập
                        if (string.IsNullOrEmpty(userId))
                            throw new BusinessRuleException("Vui lòng đăng nhập tài khoản sinh viên để tải.");
                        break;

                    case FilePermission.PublicDownload:
                        // Ai cũng tải được -> Break
                        break;
                }
            }

            // 2. Tăng lượt tải
            file.DownloadCount++;
            await _repo.UpdateAsync(file);

            // 3. Lấy Stream file
            var stream = _storage.GetFileStream(file.FilePath);
            if (stream == null) throw new NotFoundException("File vật lý", file.FileName);

            return (stream, file.ContentType, file.FileName);
        }

        public async Task<List<FileResourceDto>> SearchFilesAsync(string keyword, List<string>? subjectCodes, int page, int pageSize)
        {
            // Gọi Repo
            var entities = await _repo.SearchAsync(keyword, page, pageSize, subjectCodes);

            // Map sang DTO
            return _mapper.Map<List<FileResourceDto>>(entities);
        }
    }
}