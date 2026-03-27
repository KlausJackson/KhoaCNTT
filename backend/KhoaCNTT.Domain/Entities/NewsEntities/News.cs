using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Domain.Entities.NewsEntities
{
    public class News : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public int CurrentResourceId { get; set; }
        public NewsResource CurrentResource { get; set; } = null!;
        public int ViewCount { get; set; } = 0;
        public NewsType NewsType { get; set; }

        // Khóa ngoại Admin
        public int CreatedBy { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Admin Admin { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}