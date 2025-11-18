using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{


    public interface IUserService
    {
        public Task<AppUser> Register(string username, string password);
        public Task<AppUser?> GetByUserName(string username);

        public Task<AppUser> GetById(int id);

        Task<string> Login(string username, string password);
        public Task<IEnumerable<AppUser>> GetAll();
    }
}