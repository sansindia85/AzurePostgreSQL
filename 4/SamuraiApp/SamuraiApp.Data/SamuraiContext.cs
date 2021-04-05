using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    //DbContext provides logic for EF Core to interact with your database. Change tracking
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=sanspostgres.postgres.database.azure.com;Database=samuraiappdata;Username=sandeep@sanspostgres;Password=windows10#;SslMode=Require");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Samurai>()
                .HasMany(s => s.Battles)
                .WithMany(b => b.Samurais)
                .UsingEntity<BattleSamurai>
                (bs => bs.HasOne<Battle>().WithMany(),
                    bs => bs.HasOne<Samurai>().WithMany())
                //Let's have the database populate the payload property, DateJoined.
                .Property(bs => bs.DateJoined)
                .HasDefaultValueSql("now()");
        }
    }
}
