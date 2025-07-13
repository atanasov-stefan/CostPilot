
namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestDetailsViewModel
    {
        public string Number { get; set; } = null!;

        public string Amount { get; set; } = null!;

        public string SubmittedOn { get; set; } = null!;

        public string? DecisionOn { get; set; }

        public string Approver { get; set; } = null!;

        public string? Comment { get; set; }

        public string BriefDescription { get; set; } = null!;

        public string DetailedDescription { get; set; } = null!;

        public string Center { get; set; } = null!;

        public string Currency { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Type { get; set; } = null!;
    }
}
