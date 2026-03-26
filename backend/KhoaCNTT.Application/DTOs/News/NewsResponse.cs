using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News
{
    public class NewsResponse
    {
        public int NewsID { get; set; }
        public string Title { get; set; } = string.Empty;
        public NewsType NewsType { get; set; }
        public int ViewCount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Content { get; set; } = string.Empty;
     
    }
}