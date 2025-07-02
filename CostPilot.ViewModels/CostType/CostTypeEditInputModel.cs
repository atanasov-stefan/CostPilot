using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationConstants.CostType;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.CostType
{
    public class CostTypeEditInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = BetweenLengthErrorMessage)]
        public string Description { get; set; } = null!;
    }
}
