using KhoaCNTT.Domain.Enums;
using KhoaCNTT.Domain.Common;

namespace KhoaCNTT.Domain.Entities.NewsEntities
{
    public class NewsRequest : BaseEntity
    {
        public int? TargetNewsId { get; set; }
        public News? TargetNews { get; set; }

        public int NewResourceId { get; set; }
        public NewsResource NewResource { get; set; } = null!;

        public int? OldResourceId { get; set; }
        public NewsResource? OldResource { get; set; }

        public string Title { get; set; } = string.Empty;
        public NewsType NewsType { get; set; }

      

        public RequestType RequestType { get; set; }
        public bool IsProcessed { get; set; } = false;
    }
}