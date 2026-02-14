
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace KhoaCNTT.Infrastructure.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        // private readonly IWebHostEnvironment _env;

        //public LocalFileStorageService(IWebHostEnvironment env)
        //{
        //    _env = env;

        //}

        private readonly string _baseUploadPath;
        public LocalFileStorageService(IConfiguration config)
        {
            // Lấy đường dẫn từ config, nếu không có thì mặc định lưu vào C:/Temp/KhoaCNTT_Uploads
            _baseUploadPath = config["FileStorage:UploadFolder"] ?? @"C:\Temp\KhoaCNTT_Uploads";
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            // Tạo tên file ngẫu nhiên
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(_baseUploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // trong DB chỉ lưu tên, khi dùng thì ghép _baseUploadPath + tên
            return fileName;
        }

        public FileStream GetFileStream(string fileNameInDb)
        {
            var fullPath = Path.Combine(_baseUploadPath, fileNameInDb); // Ghép với tên file

            if (!File.Exists(fullPath)) return null;

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        }

        public Task DeleteFileAsync(string fileNameInDb)
        {
            var fullPath = Path.Combine(_baseUploadPath, fileNameInDb); // Ghép với tên file
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }

    }
}