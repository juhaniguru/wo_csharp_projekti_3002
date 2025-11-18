using API.Dtos;
using API.Models;
using AutoMapper;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogDto>();
    }
}