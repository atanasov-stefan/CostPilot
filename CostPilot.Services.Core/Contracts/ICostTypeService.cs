using CostPilot.ViewModels.CostType;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostTypeService
    {
        public Task<IEnumerable<CostTypeIndexViewModel>> GetCostTypesAsync();

        public Task<bool> CreateCostTypeAsync(CostTypeCreateInputModel model);

        public Task<bool> DisableCostTypeAsync(string? id);

        public Task<bool> EnableCostTypeAsync(string? id);

        public Task<CostTypeEditInputModel?> GetCostTypeForEditAsync(string? id);

        public Task<bool> EditCostTypeAsync(CostTypeEditInputModel model);

        public Task<IEnumerable<CostTypeDetailsViewModel>> GetActiveCostTypesAsync();
    }
}
