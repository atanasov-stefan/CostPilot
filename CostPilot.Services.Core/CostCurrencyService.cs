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

        public async Task<bool> DisableCostCurrencyAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCurrencyToDisable = await this.dbContext.CostCurrencies
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCurrencyToDisable != null)
                    { 
                        operationResult = true;
                        costCurrencyToDisable.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EditCostCurrencyAsync(CostCurrencyEditInputModel model)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costCurrencyForEdit = await this.dbContext.CostCurrencies
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    var isCodeDuplicated = await this.dbContext.CostCurrencies
                        .AnyAsync(cc => cc.Code.ToLower() == model.Code.ToLower() && cc.Id != idGuid);
                    if (costCurrencyForEdit != null &&
                        isCodeDuplicated == false)
                    { 
                        operationResult = true;
                        costCurrencyForEdit.Code = model.Code;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EnableCostCurrencyAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCurrencyToEnable = await this.dbContext.CostCurrencies
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCurrencyToEnable != null)
                    { 
                        operationResult = true;
                        costCurrencyToEnable.IsDeleted = false;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
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

        public async Task<CostCurrencyEditInputModel?> GetCostCurrencyForEditAsync(string? id)
        {
            CostCurrencyEditInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCurrencyForEdit = await this.dbContext.CostCurrencies
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCurrencyForEdit != null)
                    {
                        model = new CostCurrencyEditInputModel() 
                        {
                            Id = costCurrencyForEdit.Id.ToString(),
                            Code = costCurrencyForEdit.Code
                        };
                    }
                }
            }

            return model;
        }

        private bool IsIdNullOrEmptyOrWhiteSpace(string? id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return true;
            }

            return false;
        }
    }
}
