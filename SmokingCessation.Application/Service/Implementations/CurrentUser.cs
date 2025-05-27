using System;


namespace SmokingCessation.Application.Service.Implementations
{
    public record CurrentUser(string UserId, string email, IEnumerable<string> Roles)
    {

        public bool isInRole(string role) => Roles.Contains(role);

    }
}

