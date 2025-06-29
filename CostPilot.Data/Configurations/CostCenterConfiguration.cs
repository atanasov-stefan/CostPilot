using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CostPilot.Data.Models;

namespace CostPilot.Data.Configurations
{
    public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
    {
        public void Configure(EntityTypeBuilder<CostCenter> entity)
        {
            entity.HasData(
                new CostCenter()
                {
                    Id = Guid.Parse("28afb175-cbad-4f83-8a6c-54eba351497e"),
                    Code = "1000",
                    Description = "Human Resources",
                    IsDeleted = false
                },

                new CostCenter()
                {
                    Id = Guid.Parse("55cf3a59-c1f8-4d57-9d0c-d99c9b58f50e"),
                    Code = "1001",
                    Description = "Finance & Accounting",
                    IsDeleted = false
                },

                new CostCenter()
                {
                    Id = Guid.Parse("59376db9-4aa1-4aa2-aabf-fad94933df13"),
                    Code = "1002",
                    Description = "Marketing",
                    IsDeleted = false
                });
        }
    }
}
