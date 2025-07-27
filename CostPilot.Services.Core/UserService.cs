using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using CostPilot.Data.Models;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.User;

namespace CostPilot.Services.Core
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IEnumerable<UserIndexViewModel>> GetUserFullDetailsAsync()
        {
            var users = await this.userManager.Users
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserIndexViewModel()
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Username = u.UserName!,
                    Email = u.Email!,
                    Roles = "",
                })
                .ToListAsync();

            foreach (var user in users) 
            {
                var userRoles = await this.userManager.GetRolesAsync((await this.userManager.FindByIdAsync(user.Id))!);
                if (userRoles.Count > 0)
                { 
                    user.Roles = string.Join(", ", userRoles);
                }
            }

            return users;
        }

        public async Task<IEnumerable<UserDetailsViewModel>> GetUserDetailsAsync()
        {
            var users = await this.userManager.Users
                .Where(u => u.UserName != "Admin")
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserDetailsViewModel()
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserRoleInputModel?> GetUserForRoleAssignmentOrRemovalAsync(string? id)
        {
            UserRoleInputModel? model = null;
            if (string.IsNullOrEmpty(id) == false && 
                string.IsNullOrWhiteSpace(id) == false)
            {
                var user = await this.userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model = new UserRoleInputModel()
                    {
                        Id = user.Id,
                        Role = "",
                    };
                }
            }

            return model;
        }

        public async Task<bool> AssignRoleToUserAsync(UserRoleInputModel model)
        {
            var operationResult = false;
            if (string.IsNullOrEmpty(model.Id) == false &&
                string.IsNullOrWhiteSpace(model.Id) == false)
            {
                var user = await this.userManager.FindByIdAsync(model.Id);
                var roleExists = await this.roleManager.RoleExistsAsync(model.Role);
                if (user != null && roleExists == true) 
                {
                    var userIsInRole = await this.userManager.IsInRoleAsync(user, model.Role);
                    if (userIsInRole == false)
                    {
                        var result = await userManager.AddToRoleAsync(user, model.Role);
                        operationResult = result.Succeeded;
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> RemoveRoleFromUserAsync(UserRoleInputModel model)
        {
            var operationResult = false;
            if (string.IsNullOrEmpty(model.Id) == false &&
                string.IsNullOrWhiteSpace(model.Id) == false)
            {
                var user = await this.userManager.FindByIdAsync(model.Id);
                var roleExists = await this.roleManager.RoleExistsAsync(model.Role);
                if (user != null && roleExists == true)
                {
                    var userIsInRole = await this.userManager.IsInRoleAsync(user, model.Role);
                    if (userIsInRole == true)
                    {
                        var result = await userManager.RemoveFromRoleAsync(user, model.Role);
                        operationResult = result.Succeeded;
                    }
                }
            }

            return operationResult;
        }
    }
}
