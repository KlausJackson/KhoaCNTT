using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.API.Models.News
{
    public class UpdateNewsRequest
    {
        /// <summary>ID bài viết cần thay thế nội dung.</summary>
        public int TargetNewsID { get; set; }

        /// <summary>Tiêu đề mới (tối đa 100 ký tự).</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Loại tin tức mới.</summary>
        public NewsType NewsType { get; set; }

        /// <summary>Nội dung chi tiết mới.</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Nội dung tóm tắt / URL thumbnail mới (tối đa 255 ký tự).</summary>
        public string ResourceContent { get; set; } = string.Empty;
    }
}