
using API.Dtos;
using API.Models;
using AutoMapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // tässä määritellään, mistä-> mihin mäppäys tehdään
        // eli pystymme nyt luomaan automapperin avulla automaattisesti
        // appuser-tyyppisistä instansseista UserRes-tyyppisiä instansseja
        CreateMap<AppUser, UserDto>();
    }
}