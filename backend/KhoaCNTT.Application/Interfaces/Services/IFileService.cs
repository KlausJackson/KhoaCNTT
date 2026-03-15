using KhoaCNTT.Application.DTOs.File;

public interface IFileService
{
    // --- UPLOAD & DUYỆT ---
    Task UploadFileAsync(UploadFileRequest request, string username, int adminLevel);
    Task ReplaceFileAsync(int fileId, UploadFileRequest request, string username, int adminLevel);
    Task ApproveFileAsync(int requestId, bool isApproved, string? reason, string username);
    Task<PagedResult<FileRequestDto>> GetPendingRequestsAsync();

    // --- SỬ DỤNG ---
    // Tìm kiếm/Lấy danh sách
    Task<PagedResult<FileDto>> SearchFilesAsync(string? keyword, List<string>? subjectCodes, string? fileType, int page, int pageSize, string userId, bool isAdmin);

    // Xem chi tiết (Tăng view count)
    Task<(Stream stream, string contentType)> GetFileByIdAsync(int id, string? userId, bool isAdmin);

    // Tải file (Tăng download count)
    Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(int fileId, string? userId, bool isAdmin);

    // --- Quản lý ---
    Task DeleteFileAsync(int fileId); // Xóa FileEntity
    Task UpdateFileInfoAsync(int fileId, UpdateFileRequest request); // Sửa metadata trực tiếp (Admin cấp cao)

    // --- STATS ---
    Task<Dictionary<string, int>> GetStatsByFileTypeAsync();
    Task<Dictionary<string, int>> GetStatsBySubjectAsync();
    Task<Dictionary<string, int>> GetStatsByTrafficAsync();
}