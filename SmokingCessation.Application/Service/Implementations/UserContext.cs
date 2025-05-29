
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.Exceptions;
using SmokingCessation.Application.Service.Interface;


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
         
            var user = _context.HttpContext?.User; // Use null-conditional for HttpContext
            if (user == null)
            {
                // This might indicate an issue with HttpContextAccessor setup or request context
                throw new InvalidOperationException("User context is not present in HttpContext.");
            }

            // CORRECTED LOGIC: If the user's identity is null OR they are NOT authenticated, return null.
            // Otherwise, proceed to extract claims.
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException("Need Authorization"); // User is not authenticated, so no current user to return
            }

            // Ensure claims exist before attempting to access .Value
            var userIdClaim = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            var emailClaim = user.FindFirst(c => c.Type == ClaimTypes.Email);
            var rolesClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role);

            if (userIdClaim == null || emailClaim == null)
            {
                // Log or handle cases where essential claims are missing
                throw new InvalidOperationException("Essential user claims (NameIdentifier or Email) are missing.");
            }

            var userId = userIdClaim.Value;
            var email = emailClaim.Value;
            var roles = rolesClaims.Select(c => c.Value).ToList(); // Convert to List for safety

            return new CurrentUser(userId, email, roles);
        
        }

        public string? GetUserId()
        {
            var userId = _context.HttpContext?.User.FindFirst("UserId")?.Value;
            return userId;

        }
    }
}

