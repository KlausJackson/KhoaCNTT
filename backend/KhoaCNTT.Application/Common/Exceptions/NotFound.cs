using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// khi không tìm thấy đối tượng trong cơ sở dữ liệu
namespace KhoaCNTT.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Không tìm thấy '{name}' với khóa: {key}")
        {
        }
    }
}