using CostPilot.ViewModels.Role;

namespace CostPilot.Services.Core.Contracts
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleDetailsViewModel>> GetAllRolesExceptUserRolesAsync(string? userId);

        public Task<IEnumerable<RoleDetailsViewModel>> GetUserRolesAsync(string? userId);
    }
}
