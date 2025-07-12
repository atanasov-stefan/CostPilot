using CostPilot.ViewModels.CostRequest;

namespace CostPilot.Services.Core.Contracts
{
    public interface ICostRequestService
    {
        public Task<bool> CreateCostRequestAsync(CostRequestCreateInputModel model, decimal costRequestAmount, string userId);

        public Task<IEnumerable<CostRequestIndexViewModel>?> GetMyCostRequestsAsync(string userId);
    }
}
