using Microsoft.EntityFrameworkCore;
using PensionContributionManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PensionContributionManagementSystem.Infrastructure.Contexts
{
    public class AppDbContext : IdentityDbContext<Member>
    {
    
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Benefit> Benefits { get; set; }

        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           
        }
    }
}
