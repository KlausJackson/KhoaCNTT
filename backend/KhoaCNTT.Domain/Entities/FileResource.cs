using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Domain.Entities
{
    public class FileResource : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty; // Tên file lúc upload
        public string FilePath { get; set; } = string.Empty; // Đường dẫn
        public string ContentType { get; set; } = string.Empty; // MIME type (pdf, docx...)
        public long Size { get; set; }

        public int ViewCount { get; set; } = 0;
        public int DownloadCount { get; set; } = 0;

        public FileStatus Status { get; set; } = FileStatus.Pending; // Enum
        public string? RejectReason { get; set; }
        public int? ParentFileId { get; set; }
        public FileResource? ParentFile { get; set; }

        public FilePermission Permission { get; set; } = FilePermission.PublicRead; // Enum

        // FK Subject
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        // FK Admin Created
        public int CreatedById { get; set; }
        public AdminUser CreatedBy { get; set; } = null!;

        // FK Admin Approved
        public int? ApprovedById { get; set; }
        public AdminUser? ApprovedBy { get; set; }
    }
}
