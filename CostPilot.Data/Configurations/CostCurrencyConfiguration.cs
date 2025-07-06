using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CostPilot.Data.Models;

namespace CostPilot.Data.Configurations
{
    public class CostCurrencyConfiguration : IEntityTypeConfiguration<CostCurrency>
    {
        public void Configure(EntityTypeBuilder<CostCurrency> entity)
        {
            entity.HasData(
                new CostCurrency()
                {
                    Id = Guid.Parse("8307fe38-981d-4322-821e-e805353fd152"),
                    Code = "BGN",
                    IsDeleted = false,
                },

                new CostCurrency()
                {
                    Id = Guid.Parse("a1eb3603-83fc-4c89-9e95-e0a73cd4c3ec"),
                    Code = "EUR",
                    IsDeleted = false,
                },

                new CostCurrency()
                {
                    Id = Guid.Parse("2eb6571e-2efe-4076-b9a8-ffcaaf6658dc"),
                    Code = "USD",
                    IsDeleted = false,
                });
        }
    }
}
