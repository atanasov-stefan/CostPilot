using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using CostPilot.Data;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCenter;
using CostPilot.Data.Models;

namespace CostPilot.Services.Core
{
    public class CostCenterService : ICostCenterService
    {
        private readonly CostPilotDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public CostCenterService(CostPilotDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<bool> CreateCostCenterAsync(CostCenterCreateInputModel model)
        {
            var operationResult = false;
            var isCodeDuplicated = await this.dbContext.CostCenters.AnyAsync(cc => cc.Code.ToLower() == model.Code.ToLower());
            var owner = await this.userManager.FindByIdAsync(model.OwnerId);
            if (isCodeDuplicated == false &&
                owner != null)
            {
                var costCenter = new CostCenter()
                {
                    Code = model.Code,
                    Description = model.Description,
                    OwnerId = model.OwnerId,
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
            var owner = await this.userManager.FindByIdAsync(model.OwnerId);
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false &&
                owner != null)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costCenterForEdit = await this.dbContext.CostCenters
                    .FirstOrDefaultAsync(cc => cc.Id == idGuid);
                    var isDescriptionDuplicated = await this.dbContext.CostCenters.AnyAsync(cc => cc.Description.ToLower() == model.Description.ToLower() && cc.Id != idGuid);
                    if (costCenterForEdit != null &&
                        isDescriptionDuplicated == false)
                    {
                        operationResult = true;
                        costCenterForEdit.Description = model.Description;
                        costCenterForEdit.OwnerId = model.OwnerId;
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

        public async Task<IEnumerable<CostCenterDetailsViewModel>> GetActiveCostCentersAsync()
        {
            var activeCostCenters = await this.dbContext.CostCenters
                .AsNoTracking()
                .Where(cc => cc.IsDeleted == false)
                .OrderBy(cc => cc.Code)
                .Select(cc => new CostCenterDetailsViewModel()
                {
                    Id = cc.Id.ToString(),
                    CodeDescription = $"{cc.Code} - {cc.Description}",
                })
                .ToListAsync();

            return activeCostCenters;
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
                            OwnerId = costCenterForEdit.OwnerId,
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
                    IsObsolete = cc.IsDeleted == true ? "Yes" : "No",
                    Owner = $"{cc.Owner.FirstName} {cc.Owner.LastName}"
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
