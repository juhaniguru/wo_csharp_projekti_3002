using API.CustomExceptions;
using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class BlogService(DataContext _repository) : IBlogService
    {
        public async Task<Blog> Create(CreateBlogReq requestData, int loggedInUser)
        {
           
                var blog = new Blog
                {
                    Title = requestData.Title,
                    Content = requestData.Content,
                    AppUserId = loggedInUser
                };

                await _repository.Blogs.AddAsync(blog);
                await _repository.SaveChangesAsync();

                return blog;
            
            
        }

        public async Task<IEnumerable<Blog>> GetAll()
        {
            // SELECT * FROM Blogs;
            return await _repository.Blogs.AsNoTracking().ToListAsync();
        }

        public async Task<Blog> GetById(int id)
        {
            var blogWithOwner = await _repository.Blogs.Include(b => b.Owner).FirstOrDefaultAsync(b => b.Id == id);
            if(blogWithOwner == null)
            {
                throw new NotFoundException("Blog not found");
            }

            return blogWithOwner;
        }
    }
}