using System;

using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Specifications;
using static SmokingCessation.Application.DTOs.Request.AuthenticationRequest;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IAuthenticationService
    {
        Task<BaseResponseModel> AssignUserRole(AssignUserRoles request);
        Task<BaseResponseModel<UserResponse>> RegisterAsync(UserRegisterRequest request);
        Task<BaseResponseModel<UserCurrenResponse>> GetCurrentUserAsync();
        Task<BaseResponseModel<UserResponse>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<UserCurrenResponse>> UpdateAsync(Guid id, UpdateUserRequest request);
        Task<BaseResponseModel> DeleteAsync(Guid id);
        Task<BaseResponseModel<RevokeRefreshTokenResponse>> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest);
        Task<BaseResponseModel<CurrentUserResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<BaseResponseModel<UserResponse>> LoginAsync(UserLoginRequest request);
        Task<PaginatedList<UserFullResponse>> GetAllUser(PagingRequestModel paging);

    }
}
