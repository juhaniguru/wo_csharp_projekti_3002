using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService _userService, IMapper _mapper) : ControllerBase
    {

        [HttpGet("account/rewards")]
        [Authorize(Policy = "Require1000Xp")]
        public async Task<ActionResult> GetRewards()
        {
            return await Task.FromResult(Ok());
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(
                _mapper.Map<IEnumerable<UserDto>>(users)
            );
            
        } 

        [HttpPost("login", Name = "Login user")]
        public async Task<ActionResult<LoginRes>> Login(LoginReq requestData)
        {
            try
            {
                var token = await _userService.Login(requestData.UserName, requestData.Password);
                return Ok(new LoginRes
                {
                    Token = token
                });
            } catch(NotFoundException e)
            {
                return Problem(title: "Error logging in", detail: e.Message, statusCode: StatusCodes.Status404NotFound);
            } catch(Exception e)
            {
                return Problem(title: "Error logging in", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("register", Name = "Register new user")]
        public async Task<ActionResult<RegisterRes>> Register(RegisterReq requestData)
        {
            try
            {
                var user = await _userService.Register(requestData.UserName, requestData.Password);
                return new RegisterRes
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role
                };
            }
            // toistaiseksi missään ei vielä heitetä tätä omaa poikkeusta
            // otetaan se käyttöön myöhemmin
            catch (UserRegistrationException e)
            {
                return Problem(
                    title: "Error creating user",
                    detail: e.Message,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            catch (Exception e)
            {
                return Problem(
                    title: "Error creating user",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
        
            
        
    }
}