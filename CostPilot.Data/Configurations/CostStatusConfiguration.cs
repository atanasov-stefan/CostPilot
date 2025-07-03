using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CostPilot.Data.Models;

namespace CostPilot.Data.Configurations
{
    public class CostStatusConfiguration : IEntityTypeConfiguration<CostStatus>
    {
        public void Configure(EntityTypeBuilder<CostStatus> entity)
        {
            entity.HasData(
                new CostStatus() 
                { 
                    Id = Guid.Parse("c584c84f-fcd2-464a-957e-cfd57549ccaa"),
                    Description = "Pending",
                    IsDeleted = false
                },

                new CostStatus()
                {
                    Id = Guid.Parse("6abfacab-495a-4b43-bca3-b2ba53c71d9a"),
                    Description = "Approved",
                    IsDeleted = false
                },

                new CostStatus()
                {
                    Id = Guid.Parse("f53b3520-6ebd-4d00-b280-a109ac0a0b54"),
                    Description = "Rejected",
                    IsDeleted = false
                });
        }
    }
}
