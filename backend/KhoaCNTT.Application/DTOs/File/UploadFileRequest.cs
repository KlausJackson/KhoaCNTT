
using KhoaCNTT.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace KhoaCNTT.Application.DTOs.File
{
    public class UploadFileRequest
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!; // File gửi lên
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public FilePermission Permission { get; set; }
    }
}