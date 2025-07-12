using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static CostPilot.Common.ValidationConstants.CostRequest;

namespace CostPilot.Data.Models
{
    public class CostRequest
    {
        [Key]
        [Comment("Cost Request Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(NumberMaxLength)]
        [Comment("Cost Request Number")]
        public string Number { get; set; } = null!;

        [Required]
        [Comment("Cost Request Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Comment("Cost Request SubmittedOn Date")]
        public DateTime SubmittedOn { get; set; }

        [Comment("Cost Request DecisionOn Date")]
        public DateTime? DecisionOn { get; set; }

        [Required]
        [Comment("Foreign Key Reference To Application User - Requestor")]
        public string RequestorId { get; set; } = null!;

        [ForeignKey(nameof(RequestorId))]
        public virtual ApplicationUser Requestor { get; set; } = null!;

        [Required]
        [Comment("Foreign Key Reference To Application User - Approver")]
        public string ApproverId { get; set; } = null!;

        [ForeignKey(nameof(ApproverId))]
        public virtual ApplicationUser Approver { get; set; } = null!;

        [StringLength(CommentMaxLength)]
        [Comment("Cost Request Approver Comment")]
        public string? Comment { get; set; }

        [Required]
        [StringLength(BriefDescriptionMaxLength)]
        [Comment("Cost Request Brief Description")]
        public string BriefDescription { get; set; } = null!;

        [Required]
        [StringLength(DetailedDescriptionMaxLength)]
        [Comment("Cost Request Detailed Description")]
        public string DetailedDescription { get; set; } = null!;

        [Required]
        [Comment("Foreign Key Reference To Cost Center")]
        public Guid CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public virtual CostCenter Center { get; set; } = null!;

        [Required]
        [Comment("Foreign Key Reference To Cost Currency")]
        public Guid CurrencyId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public virtual CostCurrency Currency { get; set; } = null!;

        [Required]
        [Comment("Foreign Key Reference To Cost Status")]
        public Guid StatusId { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual CostStatus Status { get; set; } = null!;

        [Required]
        [Comment("Foreign Key Reference To Cost Type")]
        public Guid TypeId { get; set; }

        [ForeignKey(nameof(TypeId))]
        public virtual CostType Type { get; set; } = null!;

        [Required]
        [Comment("Cost Request IsDeleted Indicator")]
        public bool IsDeleted { get; set; } = false;
    }
}
