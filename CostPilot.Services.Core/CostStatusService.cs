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
    }
}
