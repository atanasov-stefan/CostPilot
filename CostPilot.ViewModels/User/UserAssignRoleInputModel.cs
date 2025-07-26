using System.ComponentModel.DataAnnotations;

using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.User
{
    public class UserAssignRoleInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string Role { get; set; } = null!;
    }
}
