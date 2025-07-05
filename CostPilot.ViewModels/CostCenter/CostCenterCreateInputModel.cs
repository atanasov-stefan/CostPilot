using System.ComponentModel.DataAnnotations;
using CostPilot.ViewModels.User;
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

        [Required(ErrorMessage = RequireErrorMessage)]
        public string OwnerId { get; set; } = null!;

        public IEnumerable<UserDetailsViewModel> Owners { get; set; } = new List<UserDetailsViewModel>();
    }
}
