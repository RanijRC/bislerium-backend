﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public class BlogDTO
    {
        public string? Title { get; set; }
        public string? BlogImage { get; set; }
        public string? PublishedBy { get; set; }
        public string? PublishedDate { get; set; }
        public string? Comments { get; set; }
    }
}
