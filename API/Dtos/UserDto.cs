
using API.Models;

namespace API.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }

        public required string Role { get; set; }

        public int Xp {get; set;}
        //public AppUser Owner {get; set;}
    }
}