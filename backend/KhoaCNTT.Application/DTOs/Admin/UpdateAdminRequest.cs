using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KhoaCNTT.Application.DTOs.Admin
{
    public class UpdateAdminRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Level { get; set; } // Cấp độ quyền: 1, 2, 3
        public bool IsActive { get; set; } // 1: Kích hoạt, 0: Vô hiệu hóa
    }
}
