using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public record LoginResponse(bool Flag, string Message = null!, string Token = null!, int UserId = 0, string Role = null!, string username = null!);
}
