using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Entities;

namespace KhoaCNTT.Domain.Entities.NewsEntities
{
    public class NewsResource : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        // Foreign key
        public int CreatedBy { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Admin Admin { get; set; } = null!;
    }
}