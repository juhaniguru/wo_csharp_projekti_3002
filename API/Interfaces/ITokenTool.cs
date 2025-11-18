using API.Models;

namespace API.Interfaces
{
    public interface ITokenTool
    {
        public string CreateToken(AppUser user); 
    }
}