
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.Exceptions;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;


namespace SmokingCessation.Application.Service.Implementations
{
    public class UserContext(IHttpContextAccessor _context)  : IUserContext
    {



        /// <summary>
        /// Get the current user !!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>


        public CurrentUser? GetCurrentUser()
        {
            var user = _context.HttpContext?.User;
            if (user == null)
                throw new InvalidOperationException("User context is not present in HttpContext.");

            if (user.Identity == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedException("Need Authorization");

            // Lấy userId từ nhiều khả năng
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                              ?? user.FindFirst("UserId")
                              ?? user.FindFirst("sub");
            var emailClaim = user.FindFirst(ClaimTypes.Email)
                            ?? user.FindFirst("Email")
                            ?? user.FindFirst("email");
            var rolesClaims = user.Claims.Where(c =>
                c.Type == ClaimTypes.Role ||
                c.Type == "role" ||
                c.Type == "Role"
            );

            if (userIdClaim == null || emailClaim == null)
                throw new InvalidOperationException("Essential user claims (UserId/NameIdentifier or Email) are missing.");

            var userId = userIdClaim.Value;
            var email = emailClaim.Value;
            var roles = rolesClaims.Select(c => c.Value).ToList();

            return new CurrentUser(userId, email, roles);
        }

        public string? GetUserId()
        {
            var userId = _context.HttpContext?.User.FindFirst("UserId")?.Value;
            return userId;

        }
       
    }
}

