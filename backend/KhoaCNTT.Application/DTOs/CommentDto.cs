using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhoaCNTT.Application.DTOs
{

    public class CommentResponse
    {
        public int CommentID { get; set; }
        public string Content { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
