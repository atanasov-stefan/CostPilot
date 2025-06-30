using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationErrorMessages;
using static CostPilot.Common.ValidationConstants.CostCenter;

namespace CostPilot.ViewModels.CostCenter
{
    public class CostCenterEditInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = BetweenLengthErrorMessage)]
        public string Description { get; set; } = null!;
    }
}
