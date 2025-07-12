using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Required]
        [Comment("Foreign Key Reference To Application User")]
        public string OwnerId { get; set; } = null!;

        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; } = null!;

        public virtual ICollection<CostRequest> CostRequests { get; set; } = new HashSet<CostRequest>();
    }
}
