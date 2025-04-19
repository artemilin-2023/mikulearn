using HackBack.Application.Constants;
using System.Security.Claims;

namespace HackBack.Application.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetId (this ClaimsPrincipal principal)
        {
            string? userId = principal.FindFirst(CustomClaimTypes.UserId)?.Value;

            if (string.IsNullOrEmpty(userId) || userId == "0")
            {
                throw new ArgumentNullException("The user was not found");
            }

            return Guid.Parse(userId);
        }
    }
}
