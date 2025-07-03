using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostType;
using Microsoft.EntityFrameworkCore;

namespace CostPilot.Services.Core
{
    public class CostTypeService : ICostTypeService
    {
        private readonly CostPilotDbContext dbContext;

        public CostTypeService(CostPilotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCostTypeAsync(CostTypeCreateInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostTypes.AnyAsync(ct => ct.Code.ToLower() == model.Code.ToLower()) == false)
            {
                var costType = new CostType()
                {
                    Code = model.Code,
                    Description = model.Description,
                };

                operationResult = true;
                await this.dbContext.CostTypes.AddAsync(costType);
                await this.dbContext.SaveChangesAsync();
            }

            return operationResult;
        }

        public async Task<bool> DisableCostTypeAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costTypeToDisable = await this.dbContext.CostTypes
                        .FirstOrDefaultAsync(ct => ct.Id == idGuid);
                    if (costTypeToDisable != null) 
                    {
                        operationResult = true;
                        costTypeToDisable.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EditCostTypeAsync(CostTypeEditInputModel model)
        {
            var operationResult = false;
            if (await this.dbContext.CostTypes.AnyAsync(ct => ct.Description.ToLower() == model.Description.ToLower()) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                { 
                    var costTypeForEdit = await this.dbContext.CostTypes
                        .FirstOrDefaultAsync(ct => ct.Id == idGuid);
                    if (costTypeForEdit != null)
                    {
                        operationResult = true;
                        costTypeForEdit.Description = model.Description;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> EnableCostTypeAsync(string? id)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costTypeToEnable = await this.dbContext.CostTypes
                        .FirstOrDefaultAsync(ct => ct.Id == idGuid);
                    if (costTypeToEnable != null)
                    { 
                        operationResult = true;
                        costTypeToEnable.IsDeleted = false;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<CostTypeEditInputModel?> GetCostTypeForEditAsync(string? id)
        {
            CostTypeEditInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costTypeForEdit = await this.dbContext.CostTypes
                        .FirstOrDefaultAsync(ct => ct.Id == idGuid);
                    if (costTypeForEdit != null)
                    {
                        model = new CostTypeEditInputModel() 
                        { 
                            Id = costTypeForEdit.Id.ToString(),
                            Description = costTypeForEdit.Description,
                        };
                    }
                }
            }

            return model;
        }

        public async Task<IEnumerable<CostTypeIndexViewModel>> GetCostTypesAsync()
        {
            var costTypes = await this.dbContext.CostTypes
                .AsNoTracking()
                .OrderBy(ct => ct.Code)
                .Select(ct => new CostTypeIndexViewModel()
                {
                    Id = ct.Id.ToString(),
                    Code = ct.Code,
                    Description = ct.Description,
                    IsObsolete = ct.IsDeleted == true ? "Yes" : "No"
                })
                .ToListAsync();

            return costTypes;
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
