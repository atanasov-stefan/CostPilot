using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationConstants.CostCurrency;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.CostCurrency
{
    public class CostCurrencyEditInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(CodeMaxLength, MinimumLength = CodeMinLength, ErrorMessage = ExactLengthErrorMessage)]
        public string Code { get; set; } = null!;
    }
}
