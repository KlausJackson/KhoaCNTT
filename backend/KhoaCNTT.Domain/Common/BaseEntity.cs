
using System.ComponentModel.DataAnnotations;

namespace KhoaCNTT.Domain.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; } // Khóa chính tự tăng
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Xóa mềm 
    }
}