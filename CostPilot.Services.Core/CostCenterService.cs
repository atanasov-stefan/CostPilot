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
            if (this.dbContext.CostCenters.Any(cc => cc.Code == model.Code) == false &&
                this.dbContext.CostCenters.Any(cc => cc.Description == model.Description) == false)
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

        public async Task<bool> DeleteCostCenterAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                Guid idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costCenterToDelete = await this.dbContext.CostCenters
                        .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    if (costCenterToDelete != null) 
                    {
                        operationResult = true;
                        costCenterToDelete.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
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
