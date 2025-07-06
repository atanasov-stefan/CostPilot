using CostPilot.ViewModels.CostCurrency;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostCurrencyService
    {
        public Task<IEnumerable<CostCurrencyIndexViewModel>> GetCostCurrenciesAsync();

        public Task<bool> CreateCostCurrencyAsync(CostCurrencyCreateInputModel model);
    }
}
