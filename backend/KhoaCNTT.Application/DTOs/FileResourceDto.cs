
using KhoaCNTT.Domain.Enums;

// những thông tin gửi cho client
namespace KhoaCNTT.Application.DTOs.File
{
    public class FileResourceDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public FilePermission Permission { get; set; }
        public int ViewCount { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUsername { get; set; }
    }
}