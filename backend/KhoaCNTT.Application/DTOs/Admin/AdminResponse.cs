using System;

namespace KhoaCNTT.Application.DTOs.Admin
{
    public class AdminResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int isActive { get; set; }
    }
}
