using Bislerium.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Repository.Contracts
{
    public interface IBlog
    {
        Task<BlogResponse> CreateBlogAsync(BlogDTO blogDTO);
        Task<BlogResponse> UpdateBlogAsync(int blogId, BlogDTO blogDTO);
        Task<BlogResponse> DeleteBlogAsync(int blogId);
        
    }
}
