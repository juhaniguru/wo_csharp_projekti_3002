using API.Dtos;
using API.Interfaces;
using API.Models;

namespace APITests.Fakes
{
    public class FakeBlogService : IBlogService
    {

        private readonly IEnumerable<Blog> _blogs;
        private readonly bool _shouldThrow;

        public FakeBlogService(IEnumerable<Blog> blogs, bool shouldThrow) 
        {
            _blogs = blogs;
            _shouldThrow = shouldThrow;
        }

        public Task<Blog> Create(CreateBlogReq requestData, int loggedInUser)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> Edit(int id, UpdateBlogReq requestData)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetAll()
        {
            if (_shouldThrow)
            {
                throw new Exception("Simulated service error.");
            }
            return Task.FromResult(_blogs);
        }

        public Task<Blog> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}