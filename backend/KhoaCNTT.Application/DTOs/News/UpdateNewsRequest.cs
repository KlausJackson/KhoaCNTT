using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News
{
    public class UpdateNewsRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NewsType NewsType { get; set; }
    }
}