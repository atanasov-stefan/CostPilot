using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostStatus;
using CostPilot.Data.Models;

namespace CostPilot.Services.Core
{
    public class CostStatusService : ICostStatusService
    {
        private readonly CostPilotDbContext dbContext;

        public CostStatusService(CostPilotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCostStatusAsync(CostStatusCreateInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostStatuses.AnyAsync(cs => cs.Description.ToLower() == model.Description.ToLower()) == false)
            {
                var costStatus = new CostStatus() 
                {
                    Description = model.Description,
                };

                operationResult = true;
                await this.dbContext.CostStatuses.AddAsync(costStatus);
                await this.dbContext.SaveChangesAsync();
            }

            return operationResult;
        }

        public async Task<bool> DisableCostStatusAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costStatusToDisable = await this.dbContext.CostStatuses
                        .FirstOrDefaultAsync(cs => cs.Id == idGuid);
                    if (costStatusToDisable != null) 
                    {
                        operationResult = true;
                        costStatusToDisable.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EditCostStatusAsync(CostStatusEditInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostStatuses.AnyAsync(cs => cs.Description.ToLower() == model.Description.ToLower()) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costStatusForEdit = await this.dbContext.CostStatuses
                        .FirstOrDefaultAsync(cs => cs.Id == idGuid);
                    if (costStatusForEdit != null)
                    {
                        operationResult = true;
                        costStatusForEdit.Description = model.Description;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EnableCostStatusAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costStatusToEnable = await this.dbContext.CostStatuses
                        .FirstOrDefaultAsync(cs => cs.Id == idGuid);
                    if (costStatusToEnable != null)
                    { 
                        operationResult = true;
                        costStatusToEnable.IsDeleted = false;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<IEnumerable<CostStatusIndexViewModel>> GetCostStatusesAsync()
        {
            var costStatuses = await this.dbContext.CostStatuses
                .AsNoTracking()
                .OrderBy(cs => cs.Description)
                .Select(cs => new CostStatusIndexViewModel() 
                {
                    Id = cs.Id.ToString(),
                    Description = cs.Description,
                    IsObsolete = cs.IsDeleted == true ? "Yes" : "No",
                })
                .ToListAsync();

            return costStatuses;
        }

        public async Task<CostStatusEditInputModel?> GetCostStatusForEditAsync(string? id)
        {
            CostStatusEditInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                { 
                    var costStatusForEdit = await this.dbContext.CostStatuses
                        .FirstOrDefaultAsync(cs => cs.Id == idGuid);
                    if (costStatusForEdit != null)
                    {
                        model = new CostStatusEditInputModel() 
                        { 
                            Id = costStatusForEdit.Id.ToString(),
                            Description = costStatusForEdit.Description,
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
