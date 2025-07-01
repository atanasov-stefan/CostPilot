using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using static CostPilot.Common.ValidationConstants.CostType;

namespace CostPilot.Data.Models
{
    public class CostType
    {
        [Key]
        [Comment("Cost Type Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(CodeMaxLength)]
        [Comment("Cost Type Code")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength)]
        [Comment("Cost Type Description")]
        public string Description { get; set; } = null!;

        [Required]
        [Comment("Cost Type IsDeleted Indicator")]
        public bool IsDeleted { get; set; } = false;
    }
}
