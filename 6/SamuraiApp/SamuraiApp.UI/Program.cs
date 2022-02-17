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
            //GetSamurais("After Add:");

            //QueryFilters();

            //QueryAggregates();
            #endregion

            //RetrieveAndUpdateSamurai();

            //RetrieveAndUpdateMultipleSamurais();

            //MultipleDatabaseOperations();

            //RetrieveAndDeleteSamurai();

            //QueryAndUpdateBattlesDisconnected();

            #region Inserting realated data

            //InsertNewSamuraiWithAQuote();

            //InsertNewSamuraiWithManyQuotes();

            //AddQuoteToExistingSamuraiWhileTracked();
            //AddQuoteToExistingSamuraiNotTracked(1);

            //AddQuoteToExistingSamuraiAttachNotTracked(2);

            //Simpler_AddQuoteToExistingSamuraiNotTracked(3);

            #endregion

            //Include related objects in the query.
            #region EagerLoadingRelated Data

            //Single query with left joins
            //EagerLoadSamuraiWithQuotes();

            //Query is broken up into multiple queries sent in a single command

            #endregion

            //Define the shape of query results
            #region QueryProjections
            //ProjectSomeProperties();
            #endregion

            //Loading related data for objects already in memory
            #region Explicit Load Quotes
            //ExplicitLoadQuotes();
            #endregion

            //FilteringWithRelatedData();

            //ModifyingRelatedDataWhenTracked();

            ModifyingRelatedDataWhenNotTracked();

            Console.WriteLine("Press any key...");
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes)
                .FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text += "Did you hear that again?";

            using var newContext = new SamuraiContext(); 
            //newContext.Quotes.Update(quote); //Will update all the collections.
            newContext.Entry(quote).State = EntityState.Modified; //Will update only a specific entry;
            newContext.SaveChanges();
        }
        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes)
                          .FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.SaveChanges();
        }
        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToList();

        }

        private static void ExplicitLoadQuotes()
        {
            #region Explicit loading example.

            //_context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr.Ed" });
            //_context.SaveChanges();
            //_context.ChangeTracker.Clear();
            ////------------------

            ////You can only load from a single object.
            //var samurai = _context.Samurais.Find(1);
            //_context.Entry(samurai).Collection(s => s.Quotes).Load();
            //_context.Entry(samurai).Reference(s => s.Horse).Load();

            #endregion

            #region Filter loaded data using query method
            var samurai = _context.Samurais.Find(1);
            var happyQuotes = _context.Entry(samurai)
                .Collection(s => s.Quotes)
                .Query()
                .Where(q => q.Text.Contains("happy"))
                .ToList();

            #endregion

        }

        private static void ProjectSomeProperties()
        {
            //Projecting an undefined anonymous type.
            //Returning two properties from Samurai type.
            //var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();

            //Casting a list of defined types
            // var idAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();

            //Select 2 scalars and List<Quote> from Samurai type
            //var somePropertiesWithQuotes = _context.Samurais
            //    .Select(s => new {s.Id, s.Name, s.Quotes}).ToList();

            //Select an aggregate of related data
            //var somePropertiesWithQuotes =
            //    _context.Samurais.Select(s => new {s.Id, s.Name, NumberOfQuotes = s.Quotes.Count}).ToList();

            //Filter the related data that's returned in the projected type.
            //Anonymous types are not tracked.
            //Entities that are properties of an anonymous type are tracked.
            //var somePropertiesWithQuotes = _context.Samurais
            //    .Select(s => new
            //    {
            //        s.Id, s.Name,
            //        HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
            //    }).ToList();

            //Projecting full entity objects while filtering the related objects that are also returned.
            //The debugger shows a circular reference between the objects.
            
            var samuraisAndQuotes = _context.Samurais
                .Select(s => new
                {
                    Samurai = s,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                }).ToList();

            //This will be tracked.
            //_context.ChangeTracker.Entries(), results. See in the debug view.
            var firstSamurai = samuraisAndQuotes[0].Samurai.Name += " The Happiest";
        }

        public struct IdAndName
        {
            public int Id;
            public string Name;
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            //EF Core 5 : Single query with left joins. The default behavior.
            //var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();

            //In some cases, multiple commands with separate queries may be faster
            //var splitQuery = _context.Samurais.AsSplitQuery()
            //        .Include(s => s.Quotes).ToList();

            //Filtered include is new in EF Core 5
            //Quotes will not be included for Samurais which does not have "Thanks"
            //var filterInclude = _context.Samurais
            //    .Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();

            var filterPrimaryEntityWithInclude = 
                _context.Samurais.Where(s => s.Name.Contains("Sampson"))
                .Include(s => s.Quotes).FirstOrDefault();

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

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shiamada",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I've come to save you."}
                }
            };

            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "Watch out for my sharp sword!"},
                    new Quote {Text = "I told you to watch out for the sharp sword! Oh well!"}
                }
            };

            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            //In this case, the context is tracking the Samurai at the time I added the quote.
            var samurai = _context.Samurais.FirstOrDefault();

            samurai?.Quotes.Add( new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });

            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);

            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });

            //New DbContext in disconnected scenario. Updates performance as it updates samurai.
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Update(samurai);
                newContext.SaveChanges();
            }
        }

        private static void AddQuoteToExistingSamuraiAttachNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);

            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });

            //New DbContext in disconnected scenario. Updates performance as it updates samurai.
            using (var newContext = new SamuraiContext())
            {
                //HasKey value unchanged.
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }

        }

        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote {Text = "Thanks for dinner!", SamuraiId = samuraiId};

            //C# 8 using declaration gets disposed when variable goes out of the scope.
            using var newContext = new SamuraiContext();
            newContext.Quotes.Add(quote);
            newContext.SaveChanges();
        }
    }
}
