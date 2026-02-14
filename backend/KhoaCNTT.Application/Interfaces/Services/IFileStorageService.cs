

// Để quản lý lưu trữ file (upload, xóa, lấy file stream)

using Microsoft.AspNetCore.Http;

namespace KhoaCNTT.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
        FileStream GetFileStream(string filePath);
    }
}