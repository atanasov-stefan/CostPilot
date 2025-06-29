using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using static CostPilot.Common.ValidationConstants.CostCenter;

namespace CostPilot.Data.Models
{
    public class CostCenter
    {
        [Key]
        [Comment("Cost Center Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(CodeMaxLength)]
        [Comment("Cost Center Code")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength)]
        [Comment("Cost Center Description")]
        public string Description { get; set; } = null!;

        [Required]
        [Comment("Cost Center IsDeleted Indicator")]
        public bool IsDeleted { get; set; } = false;
    }
}
