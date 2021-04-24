using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SamuraiApp.UI
{
    class Program
    {
        //Temporarily
        private static readonly SamuraiContext _context = new();
        private static readonly SamuraiContext _contextNoTracking = new SamuraiContextNoTracking();
        static async Task Main()
        {
            //At least 4 operations needed for SQL Server provider to batch commands
            //AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");

            //AddSamurais("Julie2", "Sampson2");

            //await AddVariousTypes();

            #region Without tracking
            GetSamurais("After Add:");

            QueryFilters();

            QueryAggregates();
            #endregion

            //RetrieveAndUpdateSamurai();

            //RetrieveAndUpdateMultipleSamurais();

            //MultipleDatabaseOperations();

            //RetrieveAndDeleteSamurai();

            //QueryAndUpdateBattlesDisconnected();

            Console.WriteLine("Press any key...");
        }

        private static void QueryAndUpdateBattlesDisconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            } //context is disposed

            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });

            //This samurai context has no tracking information

            using (var context2 = new SamuraiContext())
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }
        }
        private static void RetrieveAndDeleteSamurai()
        {
            var samurai = _context.Samurais.Find(1);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            if (samurai != null) samurai.Name += "San";
            _context.Samurais.Add(new Samurai {Name = "Shino"});
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }
        private static void QueryAggregates()
        {
            //var name = "Sampson";
            //var samurai = _context.Samurais.FirstOrDefault(s => s.Name == name);

            //var samurai = _context.Samurais.FirstOrDefault(s => s.Id == 2);

            //Not a LINQ method. It is a DBSet method. It executes immediately.
            //If key is found in change tracker, avoids unneeded database query.
            //var samurai = _context.Samurais.Find(2);

            #region No tracking context
            var samurai = _contextNoTracking.Samurais.Find(2);
            #endregion
        }

        private static void QueryFilters()
        {
            //Does not parameterized
            //var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();

            //Parameterized
            //var name = "Sampson";
            //_context.Samurais.Where(s => s.Name == name).ToList();

            //var samurais = _context.Samurais
            //    .Where(s => EF.Functions.Like(s.Name, "J%")).ToList();

            //Paramaterized
            //var filter = "J%";
            //var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, filter)).ToList();

            #region No tracking context
            var filter = "J%";
            var samurais = _contextNoTracking.Samurais.Where(s => EF.Functions.Like(s.Name, filter)).ToList();
            #endregion

        }
        private static async Task AddVariousTypes()
        {
            _context.AddRange(
                new Samurai { Name = "Shimado" },
                new Samurai { Name = "Okamoto" },
                new Battle { Name = "Battle of Anegawa" },
                new Battle { Name = "Battle of Nagashino" });
            //_context.Samurais.AddRange(
            //    new Samurai {Name = "Shimado"},
            //    new Samurai {Name = "Okamoto"});

            //_context.Battles.AddRange(
            //    new Battle { Name = "Battle of Anegawa"},
            //    new Battle{ Name = "Battle of Nagashino"});

            await _context.SaveChangesAsync();
        }
        private static void AddSamuraisByName(params string[] names)
        {
            foreach (var name in names)
            {
                //DB Context "tracks" or "change tracks" entities.
                _context.Samurais.Add(new Samurai { Name = name });
            }

            _context.SaveChanges();
        }
        private static void AddSamurais(params string[] names)
        {
            foreach (var name in names)
            {
                //DB Context "tracks" or "change tracks" entities.
                _context.Samurais.Add(new Samurai {Name = name});
            }

            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            //In memory collection of Samurais that keeps track of.
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            //Query Tags : EF Core will add a comment to the generated SQL.

            #region Context without tracking
            //var samurais = _context.Samurais
            //    .TagWith("ConsoleApp.Program.GetSamurais method")
            //    .ToList();
            //Console.WriteLine($"{text}: Samurai  count is {samurais.Count}");

            //foreach (var samurai in samurais)
            //{
            //    Console.WriteLine(samurai.Name);
            //}
            #endregion

            #region Context with no tracking
            var samurais = _contextNoTracking.Samurais
                .TagWith("ConsoleApp.Program.GetSamurais method")
                .ToList();
            Console.WriteLine($"{text}: Samurai  count is {samurais.Count}");

            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
            #endregion
        }
    }
}
