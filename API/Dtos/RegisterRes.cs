
namespace API.Dtos
{
    public class RegisterRes
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        
        public required string Role { get; set; }
    }
}