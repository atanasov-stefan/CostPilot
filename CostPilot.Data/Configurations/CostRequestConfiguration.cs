using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CostPilot.Data.Models;

namespace CostPilot.Data.Configurations
{
    public class CostRequestConfiguration : IEntityTypeConfiguration<CostRequest>
    {
        public void Configure(EntityTypeBuilder<CostRequest> entity)
        {
            entity
                .Property(cr => cr.Amount)
                .HasColumnType("decimal(30,2)");

            entity
                .HasOne(cr => cr.Center)
                .WithMany(cc => cc.CostRequests)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(cr => cr.Currency)
                .WithMany(cc => cc.CostRequests)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(cr => cr.Type)
                .WithMany(ct => ct.CostRequests)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(cr => cr.Status)
                .WithMany(cs => cs.CostRequests)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(cr => cr.Approver)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(cr => cr.Requestor)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
