using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.DTOs
{
    public class AdminDashboardCountsDTO
    {
        public int AllPostsCount { get; set; } 
        public int AllUpvotesCount { get; set; } 
        public int AllDownvotesCount { get; set; } 
        public int AllCommentsCount { get; set; } 
        public int MonthPostsCount { get; set; }
        public int MonthUpvotesCount { get; set; }
        public int MonthDownvotesCount { get; set; }
        public int MonthCommentsCount { get; set;}
    }
}
