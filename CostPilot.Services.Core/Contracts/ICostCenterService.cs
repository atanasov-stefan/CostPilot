using CostPilot.ViewModels.CostCenter;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostCenterService
    {
        public Task<IEnumerable<CostCenterIndexViewModel>> GetCostCentersAsync();

        public Task<bool> CreateCostCenterAsync(CostCenterCreateInputModel model);

        public Task<bool> DeleteCostCenterAsync(string? id);
    }
}
