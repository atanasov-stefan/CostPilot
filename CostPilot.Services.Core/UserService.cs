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

        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserDetailsViewModel>> GetUserDetailsAsync()
        {
            var users = await this.userManager.Users
                .Select(u => new UserDetailsViewModel()
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            return users;
        }
    }
}
