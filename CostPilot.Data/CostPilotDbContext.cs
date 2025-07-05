using CostPilot.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CostPilot.Data
{
    public class CostPilotDbContext : IdentityDbContext<ApplicationUser>
    {
        public CostPilotDbContext(DbContextOptions<CostPilotDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CostCenter> CostCenters { get; set; }

        public virtual DbSet<CostType> CostTypes { get; set; }

        public virtual DbSet<CostStatus> CostStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
