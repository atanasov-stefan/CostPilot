using CostPilot.ViewModels.Role;

namespace CostPilot.Services.Core.Contracts
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleDetailsViewModel>> GetAllRolesAsync();

        public Task<IEnumerable<RoleDetailsViewModel>> GetUserRolesAsync(string? userId);
    }
}
