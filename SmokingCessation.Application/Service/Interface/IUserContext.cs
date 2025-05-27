using System;
using SmokingCessation.Application.Service.Implementations;


namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
        

    }
}
