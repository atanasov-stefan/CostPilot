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
            if (await this.dbContext.CostTypes.AnyAsync(ct => ct.Code == model.Code) == false)
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
    }
}
