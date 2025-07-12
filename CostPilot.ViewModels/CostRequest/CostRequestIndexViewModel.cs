
namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestIndexViewModel
    {
        public string Id { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string SubmittedOn { get; set; } = null!;

        public string Amount { get; set; } = null!;

        public string Currency { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string ApproverFullName { get; set; } = null!;

        public bool IsApprovedOrRejected { get; set; }
    }
}
