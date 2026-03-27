namespace KhoaCNTT.API.Models.Comment
{
    public class CommentResponse
    {
        public int CommentID { get; set; }
        public int NewsId { get; set; }
        public string MSV { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}