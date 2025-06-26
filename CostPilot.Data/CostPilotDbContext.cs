using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CostPilot.Data
{
    public class CostPilotDbContext : IdentityDbContext
    {
        public CostPilotDbContext(DbContextOptions<CostPilotDbContext> options)
            : base(options)
        {
        }
    }
}
