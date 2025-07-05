using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CostPilot.Common.ValidationConstants.ApplicationUser;

namespace CostPilot.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(NameMaxLength)]
        [Comment("User First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(NameMaxLength)]
        [Comment("User Last Name")]
        public string LastName { get; set; } = null!;
    }
}
