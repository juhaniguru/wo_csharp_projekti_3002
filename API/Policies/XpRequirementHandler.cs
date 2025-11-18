using System.IdentityModel.Tokens.Jwt;
using API.CustomClaims;
using API.CustomExceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Policies
{
    public class XpAuthorizationHandler(IUserService _userService) : AuthorizationHandler<XpRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, XpRequirement requirement)
        {
            var xpClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == XpClaim.XP);
            if(xpClaim == null)
            {
                return;
            }
            var xpValueStr = xpClaim.Value;
            var success = int.TryParse(xpValueStr, out int xpValueInt);
            if(success)
            {
                var idClaim = context.User.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub);
                var id = int.Parse(idClaim.Value);
                try
                {
                    var account = await _userService.GetById(id);
                    if(account.Xp >= xpValueInt)
                    {
                        context.Succeed(requirement);
                    }

                } catch(NotFoundException)
                {
                    return;
                } catch(Exception)
                {
                    return;
                }
            }

            return;
        }
    }
}