using API.Dtos;
using API.Models;

namespace API.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<Blog>> GetAll();
        Task<Blog> Create(CreateBlogReq requestData, int loggedInUser);

        public Task<Blog> GetById(int id);

        Task<Blog> Edit(int id, UpdateBlogReq requestData);

        Task Remove(int id);
    }
}