using CostPilot.Data.Models;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CostPilot.Services.Core
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RoleService(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<RoleDetailsViewModel>> GetAllRolesAsync()
        {
            var roles = await this.roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleDetailsViewModel()
                {
                    Id = r.Name!,
                    Name = r.Name!,
                })
                .ToListAsync();

            return roles;
        }

        public async Task<IEnumerable<RoleDetailsViewModel>> GetUserRolesAsync(string? userId)
        {
            var roles = new List<RoleDetailsViewModel>();
            if (string.IsNullOrEmpty(userId) == false &&
                string.IsNullOrWhiteSpace(userId) == false)
            {
                var user = await this.userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var userRoles = await this.userManager.GetRolesAsync(user);
                    foreach (var role in userRoles.OrderBy(x => x)) 
                    {
                        roles.Add(new RoleDetailsViewModel()
                        {
                            Id = role,
                            Name = role,
                        });
                    }
                }
            }

            return roles;
        }
    }
}
