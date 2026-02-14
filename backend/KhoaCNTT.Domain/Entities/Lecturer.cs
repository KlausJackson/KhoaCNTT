using KhoaCNTT.Domain.Common;
using KhoaCNTT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhoaCNTT.Domain.Entities
{
    public class Lecturer : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // đường dẫn ảnh đại diện
        public DegreeType Degree { get; set; } // học vị (thạc sĩ, tiến sĩ...)
        public string Position { get; set; } = string.Empty; // chức vụ (giảng viên, trưởng bộ môn...)
        // danh sách môn học tham gia giảng dạy
        // Quan hệ: Một giảng viên dạy nhiều môn
        public ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
