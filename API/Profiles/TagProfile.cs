using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Profiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>();
        }
    }
}