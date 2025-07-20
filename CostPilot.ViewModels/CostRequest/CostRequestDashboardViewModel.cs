
namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestDashboardViewModel
    {
        public int TotalRequestsCurrentDay { get; set; }

        public int TotalRequestsCurrentMonth { get; set; }

        public int TotalRequestsCurrentYear { get; set; }

        public int TotalApprovedRequestsCurrentDay { get; set; }

        public int TotalApprovedRequestsCurrentMonth { get; set; }

        public int TotalApprovedRequestsCurrentYear { get; set; }

        public int TotalRejectedRequestsCurrentDay { get; set; }

        public int TotalRejectedRequestsCurrentMonth { get; set; }

        public int TotalRejectedRequestsCurrentYear { get; set; }

        public string AverageResponseTime { get; set; } = null!;
    }
}
