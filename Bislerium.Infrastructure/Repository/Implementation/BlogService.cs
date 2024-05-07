using Bislerium.Application.DTOs;
using Bislerium.Infrastructure.Repository.Contracts;

namespace Bislerium.Infrastructure.Repository.Implementation
{
    public class BlogService : IBlog
    {
        public Task<BlogResponse> CreateBlogAsync(BlogDTO blogDTO)
        {
            throw new NotImplementedException();
        }

        public Task<BlogResponse> DeleteBlogAsync(int blogId)
        {
            throw new NotImplementedException();
        }

        public Task<BlogResponse> UpdateBlogAsync(int blogId, BlogDTO blogDTO)
        {
            throw new NotImplementedException();
        }
    }
}
