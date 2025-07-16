using CostPilot.ViewModels.CostRequest;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostRequestService
    {
        public Task<bool> CreateCostRequestAsync(CostRequestCreateInputModel model, decimal costRequestAmount, string userId);

        public Task<IEnumerable<CostRequestIndexViewModel>?> GetMyCostRequestsAsync(string userId);

        public Task<CostRequestDetailsViewModel?> GetCostRequestDetailsAsync(string? id, string userId);

        public Task<bool> CancelCostRequestAsync(string? id, string userId);

        public Task<CostRequestEditInputModel?> GetCostRequestForEditAsync(string? id, string userId);

        public Task<bool> EditCostRequestAsync(CostRequestEditInputModel model, decimal costRequestAmount, string userId);

        public Task<IEnumerable<CostRequestForApprovalViewModel>?> GetCostRequestsForApprovalAsync(string userId);
    }
}
