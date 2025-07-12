using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using static CostPilot.Common.ValidationConstants.CostStatus;

namespace CostPilot.Data.Models
{
    public class CostStatus
    {
        [Key]
        [Comment("Cost Status Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(DescriptionMaxLength)]
        [Comment("Cost Status Description")]
        public string Description { get; set; } = null!;

        [Required]
        [Comment("Cost Status IsDeleted Indicator")]
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<CostRequest> CostRequests { get; set; } = new HashSet<CostRequest>();
    }
}
