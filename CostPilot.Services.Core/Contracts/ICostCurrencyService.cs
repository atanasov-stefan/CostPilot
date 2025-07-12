using CostPilot.ViewModels.CostCurrency;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostCurrencyService
    {
        public Task<IEnumerable<CostCurrencyIndexViewModel>> GetCostCurrenciesAsync();

        public Task<bool> CreateCostCurrencyAsync(CostCurrencyCreateInputModel model);

        public Task<bool> DisableCostCurrencyAsync(string? id);

        public Task<bool> EnableCostCurrencyAsync(string? id);

        public Task<CostCurrencyEditInputModel?> GetCostCurrencyForEditAsync(string? id);

        public Task<bool> EditCostCurrencyAsync(CostCurrencyEditInputModel model);

        public Task<IEnumerable<CostCurrencyDetailsViewModel>> GetActiveCostCurrenciesAsync();
    }
}
