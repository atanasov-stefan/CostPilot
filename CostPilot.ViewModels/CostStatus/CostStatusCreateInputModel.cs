using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationConstants.CostStatus;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.CostStatus
{
    public class CostStatusCreateInputModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = BetweenLengthErrorMessage)]
        public string Description { get; set; } = null!;
    }
}
