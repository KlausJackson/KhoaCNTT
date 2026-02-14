using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string MSV { get; set; } = string.Empty;
        public string TenSV { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Khóa ngoại: Liên kết với News
        public int NewsId { get; set; }
        public News News { get; set; } = null!;
    }
}
