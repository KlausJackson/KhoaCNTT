using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.API.Models.News
{
    public class NewsResponse
    {
        public int NewsID { get; set; }
        public string Title { get; set; } = string.Empty;
        public NewsType NewsType { get; set; }
        public int ViewCount { get; set; }

        /// <summary>ID admin đã tạo bài viết.</summary>
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Nội dung hiện tại của bài viết (từ CurrentNewsResource).</summary>
        public NewsResourceResponse CurrentResource { get; set; } = null!;
    }

    public class NewsResourceResponse
    {
        public int NewsResourceID { get; set; }

        /// <summary>Nội dung tóm tắt / URL thumbnail.</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Nội dung chi tiết bài viết.</summary>
        public string DetailContent { get; set; } = string.Empty;

        public int CreatedBy { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}