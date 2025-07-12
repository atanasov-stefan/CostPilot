using CostPilot.ViewModels.CostCenter;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostCenterService
    {
        public Task<IEnumerable<CostCenterIndexViewModel>> GetCostCentersAsync();

        public Task<bool> CreateCostCenterAsync(CostCenterCreateInputModel model);

        public Task<bool> DisableCostCenterAsync(string? id);

        public Task<bool> EnableCostCenterAsync(string? id);

        public Task<CostCenterEditInputModel?> GetCostCenterForEditAsync(string? id);

        public Task<bool> EditCostCenterAsync(CostCenterEditInputModel model);

        public Task<IEnumerable<CostCenterDetailsViewModel>> GetActiveCostCentersAsync();
    }
}
