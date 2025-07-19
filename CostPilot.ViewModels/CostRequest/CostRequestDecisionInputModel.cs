using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationConstants.CostRequest;

namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestDecisionInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string Number { get; set; } = null!;

        [StringLength(CommentMaxLength)]
        public string? Comment { get; set; }
    }
}
