using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Application.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthenticationService(ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
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

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to assign role '{role.Name}' to user '{user.Email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
