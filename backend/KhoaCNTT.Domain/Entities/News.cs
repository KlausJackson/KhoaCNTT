using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Domain.Entities
{
    public class News : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // HTML
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public NewsType NewsType { get; set; }
        public string Status { get; set; } = "Pending";
        public string? RejectReason { get; set; }

        // Khóa ngoại Admin
        public int CreatedById { get; set; }
        public AdminUser CreatedBy { get; set; } = null!; // được thiết lập khi tạo tin tức

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
