using CostPilot.ViewModels.CostStatus;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostStatusService
    {
        public Task<IEnumerable<CostStatusIndexViewModel>> GetCostStatusesAsync();

        public Task<bool> CreateCostStatusAsync(CostStatusCreateInputModel model);

        public Task<bool> DisableCostStatusAsync(string? id);

        public Task<bool> EnableCostStatusAsync(string? id);

        public Task<CostStatusEditInputModel?> GetCostStatusForEditAsync(string? id);

        public Task<bool> EditCostStatusAsync(CostStatusEditInputModel model);
    }
}
