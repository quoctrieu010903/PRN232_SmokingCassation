
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.Service.Interface;


namespace SmokingCessation.Application.Service.Implementations
{
    public class UserContext(IHttpContextAccessor context) : IUserContext
    {
        /// <summary>
        /// Get the current user !!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public CurrentUser? GetCurrentUser()
        {
            var user = context.HttpContext.User;
            if (user == null)
            {
                throw new InvalidOperationException("User context is not prensent");

            }
            if (user.Identity == null || user.Identity.IsAuthenticated)
            {
                return null;
            }
            var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)!.Select(c => c.Value);
            return new CurrentUser(userId, email, roles);
        }



    }
}

