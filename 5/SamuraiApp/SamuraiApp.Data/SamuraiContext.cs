using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            //Express the category as an array even if we use only one category
            optionsBuilder.UseNpgsql(
                    "Host=postgres-db-oversea-pilot.postgres.database.azure.com;Database=samuraiappdata;Username=postgresadmin@postgres-db-oversea-pilot;Password=oversea@Pilot;SslMode=Require",
                                 options => options.MaxBatchSize(100))
                .LogTo(Console.WriteLine, new [] {DbLoggerCategory.Database.Command.Name}, LogLevel.Information)
                .EnableSensitiveDataLogging();


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
