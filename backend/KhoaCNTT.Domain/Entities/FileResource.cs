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
        public FilePermission Permission { get; set; }
        public FileStatus Status { get; set; } = FileStatus.Pending;

        public string CreatedByUsername { get; set; } = string.Empty; // Admin nào up
        public string? ApprovedByUsername { get; set; } // Admin nào duyệt
        public string? RejectReason { get; set; } // Lý do từ chối

        public int? ParentFileId { get; set; } // ID của file cũ bị thay thế (Linked List)

        // Các trường phụ
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public int DownloadCount { get; set; } = 0;
        public int ViewCount { get; set; } = 0;
    }
}
