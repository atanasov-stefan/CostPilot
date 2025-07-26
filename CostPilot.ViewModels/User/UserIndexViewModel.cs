
namespace CostPilot.ViewModels.User
{
    public class UserIndexViewModel : UserDetailsViewModel
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Roles { get; set; } = null!;
    }
}
