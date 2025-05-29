
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public AuthenticationService(ITokenService tokenService, ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper ,IUserContext userContext)
        {
            _tokenService = tokenService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task AssignUserRole(AssignUserRoles request)
        {
            _logger.LogInformation("Assign user role : {@request}", request);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidOperationException($"User with email '{request.Email}' not found.");

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role == null)
                throw new InvalidOperationException($"Role '{request.RoleName}' not found.");
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);
            }
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to assign role '{role.Name}' to user '{user.Email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            await _userManager.DeleteAsync(user);

        }

        public async Task<AuthenticationResponse.UserResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by id");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            _logger.LogInformation("User found");
            return _mapper.Map<UserResponse>(user);
        }

        

        public async Task<AuthenticationResponse.UserResponse> LoginAsync(AuthenticationRequest.UserLoginRequest request)
        {
           if (request == null)
           {
                _logger.LogError("login request is null");
                throw new ArgumentNullException(nameof(request));
           }
           var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError($"Login failed: {request.Email}");
                throw new Exception("Invalid email or password");
            }
            
           var token =  await _tokenService.GenerateToken(user);

            var refreshToken = _tokenService.GenerateRefeshToken();

            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.ResetToken = Convert.ToBase64String(refreshTokenHash);
            user.ResetTokenExpires = CoreHelper.SystemTimeNow;

            // Update user information in database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update user: {errors}", errors);
                throw new Exception($"Failed to update user: {errors}");
            }

            var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
            userResponse.AccessToken = token;
            userResponse.RefreshToken = refreshToken;

            return userResponse;

        }

        public async Task<AuthenticationResponse.CurrentUserResponse> RefreshTokenAsync(AuthenticationRequest.RefreshTokenRequest request)
        {
            _logger.LogInformation("RefreshToken");

            // Hash the incoming RefreshToken and compare it with the one stored in the database
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

            // Find user based on the refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedRefreshToken);
            if (user == null)
            {
                _logger.LogError("Invalid refresh token");
                throw new Exception("Invalid refresh token");
            }

            // Validate the refresh token expiry time
            if (user.ResetTokenExpires < CoreHelper.SystemTimeNow)
            {
                _logger.LogWarning("Refresh token expired for user ID: {UserId}", user.Id);
                throw new Exception("Refresh token expired");
            }

            // Generate a new access token
            var newAccessToken = await _tokenService.GenerateToken(user);
            _logger.LogInformation("Access token generated successfully");
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = newAccessToken;
            return currentUserResponse;
        }

        public async Task<AuthenticationResponse.UserResponse> RegisterAsync(AuthenticationRequest.UserRegisterRequest request)
        {
           var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogError("Email already exists");
                throw new Exception("Email already exists");
            }
            var newUser = _mapper.Map<ApplicationUser>(request);
            newUser.UserName = request.FullName;

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

           
            var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
            if (!roleExists)
                throw new Exception($"Role '{request.RoleName}' does not exist. Only predefined roles can be assigned.");

            var currentUser = _userContext.GetCurrentUser();
            if (request.RoleName == UserRoles.Admin && (currentUser == null || !currentUser.isInRole(UserRoles.Admin)))
                throw new Exception("Only Admins can assign Admin role.");

            // Gán role
            await _userManager.AddToRoleAsync(newUser, request.RoleName);

            _logger.LogInformation("User created with role: {Role}", request.RoleName);
            await _tokenService.GenerateToken(newUser);

            newUser.CreatedTime = CoreHelper.SystemTimeNow;
            newUser.LastUpdatedTime = CoreHelper.SystemTimeNow;

            return _mapper.Map<AuthenticationResponse.UserResponse>(newUser);
        }
             /// <summary>
             /// Generates a unique username by concatenating the first name and last name.
             /// </summary>
             /// <param name="firstName">The first name of the user.</param>
             /// <param name="lastName">The last name of the user.</param>
             /// <returns>The generated unique username.</returns>
        
        public async Task<AuthenticationResponse.RevokeRefreshTokenResponse> RevokeRefreshToken(AuthenticationRequest.RefreshTokenRequest refreshTokenRemoveRequest)
        {
            _logger.LogInformation("Revoking refresh token");

            try
            {
                // Hash the refresh token
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

                // Find the user based on the refresh token
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedRefreshToken);
                if (user == null)
                {
                    _logger.LogError("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }

                // Validate the refresh token expiry time
                if (user.ResetTokenExpires < CoreHelper.SystemTimeNow)
                {
                    _logger.LogWarning("Refresh token expired for user ID: {UserId}", user.Id);
                    throw new Exception("Refresh token expired");
                }

                // Remove the refresh token
                user.ResetToken = null;
                user.ResetTokenExpires = null;
                // Update user information in database
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user");
                    return new RevokeRefreshTokenResponse
                    {
                        Message = "Failed to revoke refresh token"
                    };
                }
                _logger.LogInformation("Refresh token revoked successfully");
                return new RevokeRefreshTokenResponse
                {
                    Message = "Refresh token revoked successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to revoke refresh token: {ex}", ex.Message);
                throw new Exception("Failed to revoke refresh token");
            }
        }

        public async Task<AuthenticationResponse.UserResponse> UpdateAsync(Guid id, AuthenticationRequest.UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }

            user.LastUpdatedTime = CoreHelper.SystemTimeNow;
            user.FullName = request.FullName;
            
            user.Email = request.Email;
          

            await _userManager.UpdateAsync(user);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is null or empty");
                throw new Exception("Invalid user ID");
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }
    }
}
