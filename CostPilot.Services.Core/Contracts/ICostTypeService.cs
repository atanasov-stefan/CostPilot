using CostPilot.ViewModels.CostType;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostTypeService
    {
        public Task<IEnumerable<CostTypeIndexViewModel>> GetCostTypesAsync();

        public Task<bool> CreateCostTypeAsync(CostTypeCreateInputModel model);
    }
}
