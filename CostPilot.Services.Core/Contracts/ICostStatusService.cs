using CostPilot.ViewModels.CostStatus;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostStatusService
    {
        public Task<IEnumerable<CostStatusIndexViewModel>> GetCostStatusesAsync();

        public Task<bool> CreateCostStatusAsync(CostStatusCreateInputModel model);
    }
}
