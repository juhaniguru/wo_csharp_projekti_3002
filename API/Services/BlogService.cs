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

                var existingTags = await _repository.Tags.Where(tag => requestData.Tags.Contains(tag.TagText)).ToListAsync();
                var existingTagTexts = existingTags.Select(tag => tag.TagText).ToList();
                var newTagTexts = requestData.Tags.Except(existingTagTexts).ToList();
                
                var newTagEntities = new List<Tag>();
                foreach (var text in newTagTexts)
                {
                    newTagEntities.Add(new Tag
                    {
                        TagText = text
                    });

                }
                // addataan kaikki uudet tagit kerralla
                await _repository.Tags.AddRangeAsync(newTagEntities);

                var allTags = existingTags.Union(newTagEntities).ToList();



           
                var blog = new Blog
                {
                    Title = requestData.Title,
                    Content = requestData.Content,
                    AppUserId = loggedInUser,
                    Tags = allTags
                };

                await _repository.Blogs.AddAsync(blog);
                await _repository.SaveChangesAsync();

                return blog;
            
            
        }

        public async Task<Blog> Edit(int id, UpdateBlogReq requestData)
        {
            

            /*

            Blog? blog = null;
            
            if(loggedInUser.Role == "admin") {
                blog = await GetById(id)
            } else {
                blog = await _repository.Blogs.Where(b => b.AppUserId == loggedInUser.Id).FirstOrDefaultAsync(b => b.Id == id)
            }
            
            */

            //;

            var blog = await GetById(id);
            
            blog.Title = requestData.Title;
            blog.Content = requestData.Content;

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
            var blogWithOwner = await _repository.Blogs.Include(b => b.Owner).Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
            if(blogWithOwner == null)
            {
                throw new NotFoundException("Blog not found");
            }

            return blogWithOwner;
        }

        public async Task Remove(int id)
        {
            var blog = await GetById(id);
            _repository.Blogs.Remove(blog);
            await _repository.SaveChangesAsync();
        }
    }
}