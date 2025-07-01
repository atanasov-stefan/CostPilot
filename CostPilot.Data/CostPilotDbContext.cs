using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using CostPilot.Data.Models;

namespace CostPilot.Data
{
    public class CostPilotDbContext : IdentityDbContext
    {
        public CostPilotDbContext(DbContextOptions<CostPilotDbContext> options)
            : base(options)
        {
        }

        public DbSet<CostCenter> CostCenters { get; set; }

        public DbSet<CostType> CostTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
