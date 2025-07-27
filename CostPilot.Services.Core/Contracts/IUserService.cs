using CostPilot.ViewModels.User;

namespace CostPilot.Services.Core.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDetailsViewModel>> GetUserDetailsAsync();

        public Task<IEnumerable<UserIndexViewModel>> GetUserFullDetailsAsync();

        public Task<UserRoleInputModel?> GetUserForRoleAssignmentOrRemovalAsync(string? id);

        public Task<bool> AssignRoleToUserAsync(UserRoleInputModel model);

        public Task<bool> RemoveRoleFromUserAsync(UserRoleInputModel model);
    }
}