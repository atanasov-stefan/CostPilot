
namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestForApprovalViewModel
    {
        public string Id { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string SubmittedOn { get; set; } = null!;

        public string Amount { get; set; } = null!;

        public string Currency { get; set; } = null!;

        public string BriefDescription { get; set; } = null!;
    }
}
