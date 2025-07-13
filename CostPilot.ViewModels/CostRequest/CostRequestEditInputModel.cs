using System.ComponentModel.DataAnnotations;

using CostPilot.ViewModels.CostCenter;
using CostPilot.ViewModels.CostCurrency;
using CostPilot.ViewModels.CostType;
using static CostPilot.Common.ValidationConstants.CostRequest;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.CostRequest
{
    public class CostRequestEditInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string Amount { get; set; } = null!;

        [Required(ErrorMessage = BriefDescriptionRequireErrorMessage)]
        [StringLength(BriefDescriptionMaxLength, MinimumLength = BriefDescriptionMinLength, ErrorMessage = BriefDescriptionBetweenLengthErrorMessage)]
        public string BriefDescription { get; set; } = null!;

        [Required(ErrorMessage = DetailedDescriptionRequireErrorMessage)]
        [StringLength(DetailedDescriptionMaxLength, MinimumLength = DetailedDescriptionMinLength, ErrorMessage = DetailedDescriptionBetweenLengthErrorMessage)]
        public string DetailedDescription { get; set; } = null!;

        [Required(ErrorMessage = CostCenterRequireErrorMessage)]
        public string CenterId { get; set; } = null!;

        public IEnumerable<CostCenterDetailsViewModel> Centers { get; set; } = new List<CostCenterDetailsViewModel>();

        [Required(ErrorMessage = CostCurrencyRequireErrorMessage)]
        public string CurrencyId { get; set; } = null!;

        public IEnumerable<CostCurrencyDetailsViewModel> Currencies { get; set; } = new List<CostCurrencyDetailsViewModel>();

        [Required(ErrorMessage = CostTypeRequireErrorMessage)]
        public string TypeId { get; set; } = null!;

        public IEnumerable<CostTypeDetailsViewModel> Types { get; set; } = new List<CostTypeDetailsViewModel>();
    }
}
