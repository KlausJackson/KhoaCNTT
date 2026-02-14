
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.File;

namespace KhoaCNTT.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task UploadFileAsync(UploadFileRequest request, int adminLevel, string username); // Upload file
        Task ApproveFileAsync(int fileId, bool isApproved, string? rejectReason, string approverName); // Duyệt file

        Task<List<FileResourceDto>> GetPendingFilesAsync(); // Lấy danh sách file đang chờ duyệt

        Task<List<FileResourceDto>> SearchFilesAsync(string keyword, List<string>? subjectCodes, int page, int pageSize);
        Task UpdateFileInfoAsync(int fileId, UpdateFileRequest request); // Cập nhật thông tin file
        Task DeleteFileAsync(int fileId); // Xóa file
        Task<FileResourceDto> GetFileByIdAsync(int id, string? userId, bool isAdmin);
        Task<(Stream, string, string)> DownloadFileAsync(int fileId, string? userId, bool isAdmin); // Tải file
    }
}