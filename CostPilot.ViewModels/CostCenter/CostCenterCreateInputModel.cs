using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationConstants.CostCenter;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.CostCenter
{
    public class CostCenterCreateInputModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(CodeMaxLength, MinimumLength = CodeMinLength, ErrorMessage = ExactLengthErrorMessage)]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = BetweenLengthErrorMessage)]
        public string Description { get; set; } = null!;

    }
}
