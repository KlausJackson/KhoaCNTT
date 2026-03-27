using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News
{

    public class NewsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty; // Tên admin
        public int ViewCount { get; set; }
        // Dùng CommentResponse thay vì Entity Comment để bảo mật
        public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>(); 
    }
}