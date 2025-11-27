using API.Controllers;
using API.Dtos;
using API.Interfaces;
using API.Models;
using APITests.Fakes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APITests.Controllers
{
    public class BlogsControllerTests
    {

        private IMapper _mapper;
        public BlogsControllerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Blog, BlogDto>();
            });
            _mapper = mapperConfig.CreateMapper();

            
        }

        [Fact]
        public async Task GetAll_UsesFakeService_ReturnsOkWithListOfBlogDtos()
        {

            var blogsToReturn = new List<Blog>
            {
                new Blog { Id = 1, Title = "Fake Blog 1", Content = "Sisältö 1" },
                new Blog { Id = 2, Title = "Fake Blog 2", Content = "Sisältö 2" }
            };

            IBlogService fakeService = new FakeBlogService(blogsToReturn, false);
            var controller = new BlogsController(fakeService, _mapper);

            var res = await controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(res.Result);
            var blogDtos = Assert.IsAssignableFrom<IEnumerable<BlogDto>>(okResult.Value);
            Assert.Equal(blogsToReturn.Count, blogDtos.Count());

        }
    }
}