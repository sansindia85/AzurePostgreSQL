using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    //DbContext provides logic for EF Core to interact with your database. Change tracking
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=sanspostgres.postgres.database.azure.com;Database=samuraiappdata;Username=sandeep@sanspostgres;Password=windows10#;SslMode=Require");
        }
    }
}
