using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CostPilot.Data.Models;

namespace CostPilot.Data.Configurations
{
    public class CostTypeConfiguration : IEntityTypeConfiguration<CostType>
    {
        public void Configure(EntityTypeBuilder<CostType> entity)
        {
            entity.HasData(
                new CostType()
                {
                    Id = Guid.Parse("c2bf674f-65c9-4865-ac0f-5b5eb8ab64d7"),
                    Code = "OC",
                    Description = "Operating Costs",
                    IsDeleted = false
                },

                new CostType()
                {
                    Id = Guid.Parse("19befbf8-9a67-4aca-9ee1-92bd7b300324"),
                    Code = "SI",
                    Description = "Sustaining Investment",
                    IsDeleted = false
                },

                new CostType()
                {
                    Id = Guid.Parse("4936a809-ee57-43a8-842c-2144457e1b90"),
                    Code = "IP",
                    Description = "Investment Project",
                    IsDeleted = false
                });
        }
    }
}
