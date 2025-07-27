using System.ComponentModel.DataAnnotations;

using CostPilot.ViewModels.Role;

using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.ViewModels.User
{
    public class UserRoleInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string Role { get; set; } = null!;

        public IEnumerable<RoleDetailsViewModel> Roles { get; set; } = new List<RoleDetailsViewModel>();
    }
}
