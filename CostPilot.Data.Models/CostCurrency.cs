using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using static CostPilot.Common.ValidationConstants.CostCurrency;

namespace CostPilot.Data.Models
{
    public class CostCurrency
    {
        [Key]
        [Comment("Cost Currency Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(CodeMaxLength)]
        [Comment("Cost Currency Code")]
        public string Code { get; set; } = null!;

        [Required]
        [Comment("Cost Currency IsDeleted Indicator")]
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<CostRequest> CostRequests { get; set; } = new HashSet<CostRequest>();
    }
}
