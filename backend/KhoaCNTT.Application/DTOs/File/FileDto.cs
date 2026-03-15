using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.File
{
    public class FileDto
    {
        public int Id { get; set; } // ID của FileEntity
        public string Title { get; set; }
        public string FileName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public FilePermission Permission { get; set; }
        public int ViewCount { get; set; }
        public int DownloadCount { get; set; }
        public FileType FileType { get; set; }

        public long Size { get; set; }
        public DateTime CreatedAt { get; set; } // Ngày upload ban đầu
        public DateTime UpdatedAt { get; set; } // Ngày cập nhật gần nhất
    }
}