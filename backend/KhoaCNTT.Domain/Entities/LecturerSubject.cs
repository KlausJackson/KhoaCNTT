using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// định nghĩa bảng liên kết nhiều-nhiều giữa Giảng viên và Môn học
namespace KhoaCNTT.Domain.Entities
{
    public class LecturerSubject
    {
        // Khóa ngoại trỏ đến Giảng viên
        public int LecturerId { get; set; }
        public Lecturer Lecturer { get; set; } = null!;

        // Khóa ngoại trỏ đến Môn học
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}
