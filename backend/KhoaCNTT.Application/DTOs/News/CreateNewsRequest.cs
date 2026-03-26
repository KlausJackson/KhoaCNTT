using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News
{
    public class CreateNewsRequest
    {
        public string Title { get; set; } = string.Empty;
        public NewsType NewsType { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ResourceContent { get; set; } = string.Empty;
    }
}