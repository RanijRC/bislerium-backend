using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public class BlogDTO
    {
        public string? Title { get; set; }
        public IFormFile? BlogImage { get; set; }
        public string? PublishedBy { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? Comments { get; set; }
    }
}
