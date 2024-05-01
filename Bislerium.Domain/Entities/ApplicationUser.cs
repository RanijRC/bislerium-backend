using Bislerium.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public string? Firstname { get; set; }
        public string? Middlename { get; set; } = null;
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
