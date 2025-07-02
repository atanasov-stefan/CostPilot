using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCenter;
using CostPilot.Data.Models;

namespace CostPilot.Services.Core
{
    public class CostCenterService : ICostCenterService
    {
        private readonly CostPilotDbContext dbContext;

        public CostCenterService(CostPilotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCostCenterAsync(CostCenterCreateInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostCenters.AnyAsync(cc => cc.Code == model.Code) == false)
            {
                var costCenter = new CostCenter()
                {
                    Code = model.Code,
                    Description = model.Description,
                };

                operationResult = true;
                await this.dbContext.CostCenters.AddAsync(costCenter);
                await this.dbContext.SaveChangesAsync();
            }

            return operationResult;
        }

        public async Task<bool> DisableCostCenterAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCenterToDisable = await this.dbContext.CostCenters
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCenterToDisable != null)
                    {
                        operationResult = true;
                        costCenterToDisable.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EditCostCenterAsync(CostCenterEditInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostCenters.AnyAsync(cc => cc.Description == model.Description) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costCenterForEdit = await this.dbContext.CostCenters
                    .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCenterForEdit != null)
                    {
                        operationResult = true;
                        costCenterForEdit.Description = model.Description;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EnableCostCenterAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCenterToEnable = await this.dbContext.CostCenters
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCenterToEnable != null)
                    {
                        operationResult = true;
                        costCenterToEnable.IsDeleted = false;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<CostCenterEditInputModel?> GetCostCenterForEditAsync(string? id)
        {
            CostCenterEditInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCenterForEdit = await this.dbContext.CostCenters
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCenterForEdit != null)
                    {
                        model = new CostCenterEditInputModel()
                        {
                            Id = costCenterForEdit.Id.ToString(),
                            Description = costCenterForEdit.Description,
                        };
                    }
                }
            }

            return model;
        }

        public async Task<IEnumerable<CostCenterIndexViewModel>> GetCostCentersAsync()
        {
            var costCenters = await this.dbContext.CostCenters
                .AsNoTracking()
                .OrderBy(cc => cc.Code)
                .Select(cc => new CostCenterIndexViewModel()
                {
                    Id = cc.Id.ToString(),
                    Code = cc.Code,
                    Description = cc.Description,
                    IsObsolete = cc.IsDeleted == true ? "Yes" : "No"
                })
                .ToListAsync();

            return costCenters;
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
