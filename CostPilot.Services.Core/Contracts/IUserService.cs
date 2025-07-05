using CostPilot.ViewModels.User;

namespace CostPilot.Services.Core.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDetailsViewModel>> GetUserDetailsAsync();
    }
}