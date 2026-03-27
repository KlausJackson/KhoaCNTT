using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News
{
    public class NewsRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string RequesterName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}