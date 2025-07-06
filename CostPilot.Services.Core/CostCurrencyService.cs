using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCurrency;
using Microsoft.EntityFrameworkCore;

namespace CostPilot.Services.Core
{
    public class CostCurrencyService : ICostCurrencyService
    {
        private readonly CostPilotDbContext dbContext;

        public CostCurrencyService(CostPilotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCostCurrencyAsync(CostCurrencyCreateInputModel model)
        {
            var operationResult = false;
            var isCodeDuplicated = await this.dbContext.CostCurrencies.AnyAsync(cc => cc.Code.ToLower() == model.Code.ToLower());
            if (isCodeDuplicated == false) 
            {
                var costCurrency = new CostCurrency() 
                {
                    Code = model.Code,
                };

                operationResult = true;
                await this.dbContext.CostCurrencies.AddAsync(costCurrency);
                await this.dbContext.SaveChangesAsync();
            }

            return operationResult;
        }

        public async Task<IEnumerable<CostCurrencyIndexViewModel>> GetCostCurrenciesAsync()
        {
            var costCurrencies = await this.dbContext.CostCurrencies
                .AsNoTracking()
                .OrderBy(cc => cc.Code)
                .Select(cc => new CostCurrencyIndexViewModel() 
                {
                    Id = cc.Id.ToString(),
                    Code = cc.Code,
                    IsObsolete = cc.IsDeleted == true ? "Yes" : "No",
                })
                .ToListAsync();

            return costCurrencies;
        }
    }
}
