using CostPilot.ViewModels.CostRequest;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostRequestService
    {
        public Task<bool> CreateCostRequestAsync(CostRequestCreateInputModel model, decimal costRequestAmount, string userId);

        public Task<IEnumerable<CostRequestIndexViewModel>?> GetMyCostRequestsAsync(string userId, string? searchNumber, string? searchCurrency, string? searchStatus);

        public Task<CostRequestDetailsViewModel?> GetCostRequestDetailsAsync(string? id);

        public Task<bool> CancelCostRequestAsync(string? id, string userId);

        public Task<CostRequestEditInputModel?> GetCostRequestForEditAsync(string? id, string userId);

        public Task<bool> EditCostRequestAsync(CostRequestEditInputModel model, decimal costRequestAmount, string userId);

        public Task<IEnumerable<CostRequestForApprovalViewModel>?> GetCostRequestsForApprovalAsync(string userId);

        public Task<CostRequestDecisionInputModel?> GetCostRequestForDecisionAsync(string? id, string userId);

        public Task<bool> ApproveCostRequestAsync(CostRequestDecisionInputModel model, string userId);

        public Task<bool> RejectCostRequestAsync(CostRequestDecisionInputModel model, string userId);

        public Task<IEnumerable<CostRequestAfterDecisionViewModel>?> GetCostRequestsAfterDecisionAsync(string userId, string? searchNumber, string? searchCurrency, string? searchStatus);
    }
}
