using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostRequest;
using static CostPilot.Common.ApplicationConstants;

namespace CostPilot.Services.Core
{
    public class CostRequestService : ICostRequestService
    {
        private readonly CostPilotDbContext dbContext;

        public CostRequestService(CostPilotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCostRequestAsync(CostRequestCreateInputModel model, decimal costRequestAmount, string userId)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.CurrencyId) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.TypeId) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.CenterId) == false && 
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var costCurrency = await this.dbContext.CostCurrencies
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CurrencyId.ToLower());
                var costType = await this.dbContext.CostTypes
                    .FirstOrDefaultAsync(ct => ct.IsDeleted == false && ct.Id.ToString().ToLower() == model.TypeId.ToLower());
                var costCenter = await this.dbContext.CostCenters
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CenterId.ToLower());
                var costStatusPending = await this.dbContext.CostStatuses
                    .FirstOrDefaultAsync(cs => cs.IsDeleted == false && cs.Description.ToLower() == "pending");
                if (costCurrency != null &&
                    costType != null &&
                    costCenter != null && 
                    costStatusPending != null)
                {
                    var currentYear = DateTime.UtcNow.Year;
                    var costRequestsCounter = await this.dbContext.CostRequests
                        .CountAsync(cr => cr.SubmittedOn.Year == currentYear && cr.TypeId == costType.Id);
                    var costRequest = new CostRequest()
                    {
                        Number = $"{costType.Code}{currentYear}{(costRequestsCounter + 1):D4}",
                        Amount = costRequestAmount,
                        SubmittedOn = DateTime.UtcNow,
                        RequestorId = userId,
                        ApproverId = costCenter.OwnerId,
                        BriefDescription = model.BriefDescription,
                        DetailedDescription = model.DetailedDescription,
                        CenterId = costCenter.Id,
                        CurrencyId = costCurrency.Id,
                        StatusId = costStatusPending.Id,
                        TypeId = costType.Id,
                    };

                    operationResult = true;
                    await this.dbContext.CostRequests.AddAsync(costRequest);
                    await this.dbContext.SaveChangesAsync();
                }
            }

            return operationResult;
        }

        public async Task<IEnumerable<CostRequestIndexViewModel>?> GetMyCostRequestsAsync(string userId)
        {
            IEnumerable<CostRequestIndexViewModel>? myCostRequests = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                myCostRequests = await this.dbContext.CostRequests
                    .AsNoTracking()
                    .Where(cr => cr.IsDeleted == false && cr.RequestorId.ToLower() == userId.ToLower())
                    .OrderByDescending(cr => cr.SubmittedOn)
                    .Select(cr => new CostRequestIndexViewModel()
                    {
                        Id = cr.Id.ToString(),
                        Number = cr.Number,
                        SubmittedOn = cr.SubmittedOn.ToString(DateVisualisationFormat),
                        Amount = cr.Amount.ToString("F2"),
                        Currency = cr.Currency.Code,
                        Status = cr.Status.Description,
                        ApproverFullName = $"{cr.Approver.FirstName} {cr.Approver.LastName}",
                        IsApprovedOrRejected = cr.DecisionOn != null ? true : false,
                    })
                    .ToListAsync();
            }

            return myCostRequests;
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
