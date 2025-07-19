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

        public async Task<bool> CancelCostRequestAsync(string? id, string userId)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costRequestToCancel = await this.dbContext.CostRequests
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Status.Description.ToLower() == PendingStatusToLower &&
                        cr.Id == idGuid && cr.RequestorId.ToLower() == userId.ToLower());
                    if (costRequestToCancel != null)
                    {
                        operationResult = true;
                        costRequestToCancel.IsDeleted = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
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
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CurrencyId.ToLower());
                var costType = await this.dbContext.CostTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct => ct.IsDeleted == false && ct.Id.ToString().ToLower() == model.TypeId.ToLower());
                var costCenter = await this.dbContext.CostCenters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CenterId.ToLower());
                var costStatusPending = await this.dbContext.CostStatuses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cs => cs.IsDeleted == false && cs.Description.ToLower() == PendingStatusToLower);
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

        public async Task<bool> EditCostRequestAsync(CostRequestEditInputModel model, decimal costRequestAmount, string userId)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.CurrencyId) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.TypeId) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.CenterId) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var costCurrency = await this.dbContext.CostCurrencies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CurrencyId.ToLower());
                var costType = await this.dbContext.CostTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct => ct.IsDeleted == false && ct.Id.ToString().ToLower() == model.TypeId.ToLower());
                var costCenter = await this.dbContext.CostCenters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cc => cc.IsDeleted == false && cc.Id.ToString().ToLower() == model.CenterId.ToLower());
                var costRequestForEdit = await this.dbContext.CostRequests
                    .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Id.ToString().ToLower() == model.Id.ToLower() && cr.RequestorId.ToLower() == userId.ToLower());
                if (costCurrency != null &&
                    costType != null &&
                    costCenter != null &&
                    costRequestForEdit != null)
                {
                    if (costRequestForEdit.TypeId != costType.Id)
                    {
                        var currentYear = DateTime.UtcNow.Year;
                        var costRequestsCounter = await this.dbContext.CostRequests
                            .CountAsync(cr => cr.SubmittedOn.Year == currentYear && cr.TypeId == costType.Id);
                        costRequestForEdit.Number = $"{costType.Code}{currentYear}{(costRequestsCounter + 1):D4}";
                        costRequestForEdit.SubmittedOn = DateTime.UtcNow;
                        costRequestForEdit.TypeId = costType.Id;
                    }

                    if (costRequestForEdit.Amount != costRequestAmount)
                    {
                        costRequestForEdit.Amount = costRequestAmount;
                    }

                    if (costRequestForEdit.CenterId != costCenter.Id)
                    {
                        costRequestForEdit.ApproverId = costCenter.OwnerId;
                        costRequestForEdit.CenterId = costCenter.Id;
                    }

                    if (costRequestForEdit.BriefDescription != model.BriefDescription)
                    {
                        costRequestForEdit.BriefDescription = model.BriefDescription;
                    }

                    if (costRequestForEdit.DetailedDescription != model.DetailedDescription)
                    {
                        costRequestForEdit.DetailedDescription = model.DetailedDescription;
                    }

                    if (costRequestForEdit.CurrencyId != costCurrency.Id)
                    {
                        costRequestForEdit.CurrencyId = costCurrency.Id;
                    }

                    operationResult = true;
                    await this.dbContext.SaveChangesAsync();
                }
            }

            return operationResult;
        }

        public async Task<CostRequestDetailsViewModel?> GetCostRequestDetailsAsync(string? id, string userId)
        {
            CostRequestDetailsViewModel? costRequestDetails = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costRequest = await this.dbContext.CostRequests
                        .AsNoTracking()
                        .Include(cr => cr.Approver)
                        .Include(cr => cr.Center)
                        .Include(cr => cr.Currency)
                        .Include(cr => cr.Status)
                        .Include(cr => cr.Type)
                        .Include(cr => cr.Requestor)
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Id == idGuid && cr.RequestorId.ToLower() == userId.ToLower());
                    if (costRequest != null)
                    {
                        costRequestDetails = new CostRequestDetailsViewModel()
                        {
                            Number = costRequest.Number,
                            Amount = costRequest.Amount.ToString("F2"),
                            SubmittedOn = costRequest.SubmittedOn.ToString(DateVisualisationFormat),
                            DecisionOn = costRequest.DecisionOn != null ? costRequest.DecisionOn.Value.ToString(DateVisualisationFormat) : null,
                            Approver = $"{costRequest.Approver.FirstName} {costRequest.Approver.LastName}",
                            Comment = costRequest.Comment,
                            BriefDescription = costRequest.BriefDescription,
                            DetailedDescription = costRequest.DetailedDescription,
                            Center = costRequest.Center.Description,
                            Currency = costRequest.Currency.Code,
                            Status = costRequest.Status.Description,
                            Type = costRequest.Type.Description,
                            Requestor = $"{costRequest.Requestor.FirstName} {costRequest.Requestor.LastName}",
                        };
                    }
                }
            }

            return costRequestDetails;
        }

        public async Task<CostRequestEditInputModel?> GetCostRequestForEditAsync(string? id, string userId)
        {
            CostRequestEditInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costRequestForEdit = await this.dbContext.CostRequests
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Id == idGuid && cr.RequestorId.ToLower() == userId.ToLower());
                    if (costRequestForEdit != null)
                    {
                        model = new CostRequestEditInputModel()
                        {
                            Id = costRequestForEdit.Id.ToString(),
                            Amount = costRequestForEdit.Amount.ToString(),
                            BriefDescription = costRequestForEdit.BriefDescription,
                            DetailedDescription = costRequestForEdit.DetailedDescription,
                            CenterId = costRequestForEdit.CenterId.ToString(),
                            CurrencyId = costRequestForEdit.CurrencyId.ToString(),
                            TypeId = costRequestForEdit.TypeId.ToString(),
                        };
                    }
                }
            }

            return model;
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

        public async Task<IEnumerable<CostRequestForApprovalViewModel>?> GetCostRequestsForApprovalAsync(string userId)
        {
            IEnumerable<CostRequestForApprovalViewModel>? costRequestsForApproval = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                costRequestsForApproval = await this.dbContext.CostRequests
                    .AsNoTracking()
                    .Where(cr => cr.IsDeleted == false && cr.ApproverId.ToLower() == userId.ToLower() && 
                    cr.Status.Description.ToLower() == PendingStatusToLower)
                    .OrderByDescending(cr => cr.SubmittedOn)
                    .Select(cr => new CostRequestForApprovalViewModel()
                    {
                        Id = cr.Id.ToString(),
                        Number = cr.Number,
                        SubmittedOn = cr.SubmittedOn.ToString(DateVisualisationFormat),
                        Amount = cr.Amount.ToString("F2"),
                        Currency = cr.Currency.Code,
                        BriefDescription = cr.BriefDescription,
                    })
                    .ToListAsync();
            }

            return costRequestsForApproval;
        }

        public async Task<CostRequestDecisionInputModel?> GetCostRequestForDecisionAsync(string? id, string userId)
        {
            CostRequestDecisionInputModel? model = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid) == true)
                {
                    var costRequestForDecision = await this.dbContext.CostRequests
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Status.Description.ToLower() == PendingStatusToLower 
                        && cr.Id == idGuid && cr.ApproverId.ToLower() == userId.ToLower());
                    if (costRequestForDecision != null)
                    {
                        model = new CostRequestDecisionInputModel()
                        {
                            Id = costRequestForDecision.Id.ToString(),
                            Number = costRequestForDecision.Number,
                            Comment = costRequestForDecision.Comment,
                        };
                    }
                }
            }

            return model;
        }

        public async Task<bool> ApproveCostRequestAsync(CostRequestDecisionInputModel model, string userId)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costRequestToApprove = await this.dbContext.CostRequests
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Id == idGuid && cr.ApproverId.ToLower() == userId.ToLower());
                    var costStatusApproved = await this.dbContext.CostStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cs => cs.IsDeleted == false && cs.Description.ToLower() == ApprovedStatusToLower);
                    if (costRequestToApprove != null && 
                        costStatusApproved != null)
                    {
                        costRequestToApprove.DecisionOn = DateTime.UtcNow;
                        costRequestToApprove.StatusId = costStatusApproved.Id;
                        costRequestToApprove.Comment = model.Comment;
                        operationResult = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<bool> RejectCostRequestAsync(CostRequestDecisionInputModel model, string userId)
        {
            var operationResult = false;
            if (this.IsIdNullOrEmptyOrWhiteSpace(model.Id) == false &&
                this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                var idGuid = Guid.Empty;
                if (Guid.TryParse(model.Id, out idGuid) == true)
                {
                    var costRequestToReject = await this.dbContext.CostRequests
                        .FirstOrDefaultAsync(cr => cr.IsDeleted == false && cr.Id == idGuid && cr.ApproverId.ToLower() == userId.ToLower());
                    var costStatusRejected = await this.dbContext.CostStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cs => cs.IsDeleted == false && cs.Description.ToLower() == RejectedStatusToLower);
                    if (costRequestToReject != null &&
                        costStatusRejected != null)
                    {
                        costRequestToReject.DecisionOn = DateTime.UtcNow;
                        costRequestToReject.StatusId = costStatusRejected.Id;
                        costRequestToReject.Comment = model.Comment;
                        operationResult = true;
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }

            return operationResult;
        }

        public async Task<IEnumerable<CostRequestAfterDecisionViewModel>?> GetCostRequestsAfterDecisionAsync(string userId)
        {
            IEnumerable<CostRequestAfterDecisionViewModel>? costRequestsAfterDecision = null;
            if (this.IsIdNullOrEmptyOrWhiteSpace(userId) == false)
            {
                costRequestsAfterDecision = await this.dbContext.CostRequests
                    .AsNoTracking()
                    .Where(cr => cr.IsDeleted == false && cr.ApproverId.ToLower() == userId.ToLower() && 
                    (cr.Status.Description.ToLower() == ApprovedStatusToLower || cr.Status.Description.ToLower() == RejectedStatusToLower))
                    .OrderByDescending(cr => cr.SubmittedOn)
                    .Select(cr => new CostRequestAfterDecisionViewModel()
                    {
                        Id = cr.Id.ToString(),
                        Number = cr.Number,
                        SubmittedOn = cr.SubmittedOn.ToString(DateVisualisationFormat),
                        Amount = cr.Amount.ToString("F2"),
                        Currency = cr.Currency.Code,
                        Status = cr.Status.Description,
                    })
                    .ToListAsync();
            }

            return costRequestsAfterDecision;
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