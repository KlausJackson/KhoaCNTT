
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.File
{
    public class UpdateFileRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? SubjectCode { get; set; } = string.Empty;
        public FilePermission Permission { get; set; }
        public FileType FileType { get; set; }
    } // chỉ update metadata
}