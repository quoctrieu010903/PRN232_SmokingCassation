using System;
using SmokingCessation.Application.Service.Implementations;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;


namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
        public string? GetUserId();

    }
}
