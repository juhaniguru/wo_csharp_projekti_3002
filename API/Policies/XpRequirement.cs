using Microsoft.AspNetCore.Authorization;

namespace API.Policies
{
    public class XpRequirement : IAuthorizationRequirement
    {
        public XpRequirement(int xp)
        {
            Xp = xp;
        }

        public int Xp { get; }
    }
}