using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.API.Models.News
{
    public class CreateNewsRequest
    {
        /// <summary>Tiêu đề bài viết (tối đa 100 ký tự).</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Loại tin tức.</summary>
        public NewsType NewsType { get; set; }

        /// <summary>Nội dung chi tiết bài viết (HTML hoặc plain text).</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Nội dung tóm tắt / URL thumbnail (tối đa 255 ký tự).</summary>
        public string ResourceContent { get; set; } = string.Empty;
    }
}