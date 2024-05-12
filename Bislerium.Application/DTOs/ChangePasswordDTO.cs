using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public class ChangePasswordDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
