using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public class CommentDTO
    {
        public string? Content { get; set; } 
        public int BlogId { get; set; } 
        public int UserId { get; set; } 
        public DateTime PostedAt { get; set; }
    }
}
