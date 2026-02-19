
using KhoaCNTT.Domain.Common;

namespace KhoaCNTT.Domain.Entities
{
    public class AdminUser : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Mật khẩu đã mã hóa
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Level { get; set; } = 3; // Cấp độ quản trị (1: Root, 2: Manager, 3: Editor)
        public bool IsActive { get; set; } = true; // Trạng thái hoạt động


        public ICollection<News> NewsList { get; set; } = new List<News>();
        public ICollection<FileResource> CreatedFiles { get; set; } = new List<FileResource>();
        public ICollection<FileResource> ApprovedFiles { get; set; } = new List<FileResource>();
    }
}