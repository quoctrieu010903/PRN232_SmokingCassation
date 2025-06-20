using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Base;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using static System.Net.WebRequestMethods;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IPhotoService _photoService;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public AuthenticationService(ITokenService tokenService, ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper, IUserContext userContext, IPhotoService photoService)
        {
            _tokenService = tokenService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _userContext = userContext;
            _photoService = photoService;
        }

        public async Task<BaseResponseModel> AssignUserRole(AssignUserRoles request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
               throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found.");

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, $"Failed to remove current roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
            }

            var addResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addResult.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, $"Failed to assign role: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");

            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);
            }
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Gán role thành công");
        }

        public async Task<BaseResponseModel> DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found.");
            user.DeletedTime = CoreHelper.SystemTimeNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Failed to delete user.");

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Xóa user thành công");
        }

        public async Task<BaseResponseModel<UserResponse>> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,"User not found.");

            var userResponse = _mapper.Map<UserResponse>(user);
            return new BaseResponseModel<UserResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, userResponse, null, "Lấy thông tin user thành công");
        }

        public async Task<BaseResponseModel<UserResponse>> LoginAsync(AuthenticationRequest.UserLoginRequest request)
        {
            if (request == null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Request is null");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Invalid email or password");

            var token = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefeshToken();

            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.ResetToken = Convert.ToBase64String(refreshTokenHash);
            user.ResetTokenExpires = CoreHelper.SystemTimeNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Failed to update user");

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.AccessToken = token;
            return new BaseResponseModel<UserResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, userResponse, null, "Đăng nhập thành công");
        }

        public async Task<BaseResponseModel<CurrentUserResponse>> RefreshTokenAsync(AuthenticationRequest.RefreshTokenRequest request)
        {
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedRefreshToken);
            if (user == null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Invalid refresh token");

            if (user.ResetTokenExpires < CoreHelper.SystemTimeNow)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Refresh token expired");

            var newAccessToken = await _tokenService.GenerateToken(user);
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = newAccessToken;
            return new BaseResponseModel<CurrentUserResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, currentUserResponse, null, "Làm mới token thành công");
        }

        public async Task<BaseResponseModel<UserResponse>> RegisterAsync(AuthenticationRequest.UserRegisterRequest request)
        {
            string userId = _userContext.GetUserId();
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Email already exists");

            var (username, fullName) = GenerateUsernameAndFullNameFromEmail(request.Email);
            var existingUserName = await _userManager.FindByNameAsync(username);
            if (existingUserName != null)
              throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Username already exists.");

            var newUser = _mapper.Map<ApplicationUser>(request);
            newUser.UserName = username;
            newUser.FullName = fullName;
            newUser.ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg";
            newUser.CreatedTime = CoreHelper.SystemTimeNow;
            newUser.LastUpdatedTime = CoreHelper.SystemTimeNow;
            newUser.CreatedBy = userId;
            newUser.LastUpdatedBy = userId;

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(newUser, UserRoles.Member);
            await _tokenService.GenerateToken(newUser);

            var userResponse = _mapper.Map<UserResponse>(newUser);
            return new BaseResponseModel<UserResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, userResponse, null, "Đăng ký thành công");
        }

        public (string username, string fullName) GenerateUsernameAndFullNameFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ErrorException(StatusCodes.Status400BadRequest , ResponseCodeConstants.INVALID_INPUT,"Invalid email");

            var localPart = email.Split('@')[0];
            var username = localPart.Replace(".", "").ToLowerInvariant();
            var nameParts = System.Text.RegularExpressions.Regex.Split(localPart, @"(?<=\D)(?=\d)|[.\-_]");
            var fullName = string.Join(" ", nameParts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower())
            );
            return (username, fullName);
        }

        public async Task<BaseResponseModel<RevokeRefreshTokenResponse>> RevokeRefreshToken(AuthenticationRequest.RefreshTokenRequest refreshTokenRemoveRequest)
        {
            try
            {
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedRefreshToken);
                if (user == null)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED,  "Invalid refresh token" );

                if (user.ResetTokenExpires < CoreHelper.SystemTimeNow)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED,  "Refresh token expired" );

                user.ResetToken = null;
                user.ResetTokenExpires = null;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED,  "Failed to revoke refresh token" );
                }
                return new BaseResponseModel<RevokeRefreshTokenResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, new RevokeRefreshTokenResponse { Message = "Refresh token revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to revoke refresh token: {ex}", ex.Message);
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.FAILED,"Failed to revoke refresh token" );
            }
        }

        public async Task<BaseResponseModel<UserCurrenResponse>> UpdateAsync(Guid id, AuthenticationRequest.UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");
            string? imageUrl = null;

            if (request.imageUrl != null)
            {
                imageUrl = await _photoService.UploadPhotoAsync(request.imageUrl);
            }
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.ImageUrl = imageUrl; 

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Failed to update user");

            var userResponse = _mapper.Map<UserCurrenResponse>(user);
            return new BaseResponseModel<UserCurrenResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, userResponse, null, "Cập nhật user thành công");
        }

        public async Task<BaseResponseModel<CurrentUserResponse>> GetCurrentUserAsync()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.FAILED, "Invalid user ID");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            var response = _mapper.Map<CurrentUserResponse>(user);
            return new BaseResponseModel<CurrentUserResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, response, null, "Lấy thông tin user thành công");
        }
    }
}
