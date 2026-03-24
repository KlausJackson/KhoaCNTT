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
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Level { get; set; }
        public bool? IsActive { get; set; }
    }
}
