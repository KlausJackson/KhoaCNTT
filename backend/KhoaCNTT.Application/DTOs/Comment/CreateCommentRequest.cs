using System.ComponentModel.DataAnnotations;

namespace KhoaCNTT.API.Models.Comment
{
    public class CreateCommentRequest
    {
        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        public string Content { get; set; } = string.Empty;

        // Gửi từ frontend sau khi sinh viên đăng nhập hệ thống trường
        // (backend không thể decode token GUID qua JwtBearer)
        public string? Msv { get; set; }
        public string? StudentName { get; set; }
    }
}