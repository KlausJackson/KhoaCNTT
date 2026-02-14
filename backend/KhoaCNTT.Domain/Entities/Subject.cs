using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using KhoaCNTT.Domain.Common;

namespace KhoaCNTT.Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectCode { get; set; } = string.Empty; // Mã môn (vd: CSE481)
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; } // Số tín chỉ

        // Quan hệ: Một môn học được dạy bởi nhiều giảng viên
        public ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
    }
}