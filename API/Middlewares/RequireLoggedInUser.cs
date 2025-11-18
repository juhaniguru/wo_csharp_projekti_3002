
using System.IdentityModel.Tokens.Jwt;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares
{
    public class RequireLoggedInUserMiddleware(IUserService _userService) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            if(endpoint == null)
            {
                await next(context);
                return;
            }
            // [Authorize]
            var authAttr = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
            if(authAttr == null)
            {
                await next(context);
                return;
            }

            var idClaim = context.User.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var id = int.Parse(idClaim.Value);
            
            var user = await _userService.GetById(id);
            context.Items["loggedInUser"] = user;
            

            await next(context);
        }
    }
}