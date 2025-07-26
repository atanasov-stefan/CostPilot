using CostPilot.ViewModels.User;

namespace CostPilot.Services.Core.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDetailsViewModel>> GetUserDetailsAsync();

        public Task<IEnumerable<UserIndexViewModel>> GetUserFullDetailsAsync();

        public Task<UserAssignRoleInputModel?> GetUserForRoleAssignmentAsync(string? id);

        public Task<bool> AssignRoleToUserAsync(UserAssignRoleInputModel model);
    }
}