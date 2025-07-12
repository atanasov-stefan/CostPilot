
namespace CostPilot.Common
{
    public static class ValidationErrorMessages
    {
        public const string RequireErrorMessage = "Field {0} is required";
        public const string OwnerRequireErrorMessage = "Field Owner is required";
        public const string BriefDescriptionRequireErrorMessage = "Field Brief Description is required";
        public const string DetailedDescriptionRequireErrorMessage = "Field Detailed Description is required";
        public const string CostCenterRequireErrorMessage = "Field Cost Center is required";
        public const string CostTypeRequireErrorMessage = "Field Cost Type is required";
        public const string CostCurrencyRequireErrorMessage = "Field Currency is required";
        public const string ExactLengthErrorMessage = "Field {0} must be exactly {1} characters";
        public const string BetweenLengthErrorMessage = "Field {0} must be between {2} and {1} characters";
        public const string BriefDescriptionBetweenLengthErrorMessage = "Field Brief Description must be between {2} and {1} characters";
        public const string DetailedDescriptionBetweenLengthErrorMessage = "Field Detailed Description must be between {2} and {1} characters";
        public const string CreateEditOverallErrorMessage = "Fatal error occurred during saving! Please check you input!";
    }
}
