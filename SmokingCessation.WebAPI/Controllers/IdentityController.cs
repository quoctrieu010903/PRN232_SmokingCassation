
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using static SmokingCessation.Application.DTOs.Request.AuthenticationRequest;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public IdentityController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("userRole")]
         public async Task<IActionResult> AsignUserRole(AssignUserRoles request)
        {
            await _service.AssignUserRole(request);
            return Ok();
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var response = await _service.RegisterAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="request">The user login request.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var response = await _service.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        //[AllowAnonymous]
        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Refreshes the access token using the refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _service.RefreshTokenAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Revokes the refresh token.
        /// </summary>
        /// <param name="request">The refresh token request to be revoked.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost("revoke-refresh-token")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _service.RevokeRefreshToken(request);
            if (response != null && response.Message == "Refresh token revoked successfully")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
           
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }
            var response = await _service.GetCurrentUserAsync();
            return Ok(response);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The ID of the user to be deleted.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpDelete("user/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

    }
}
