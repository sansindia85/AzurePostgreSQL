using System;
using System.Linq;
using WiredBrainCoffeeShops.Data;

namespace DemoStarter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new CoffeeShopContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            LazyLoadLocationEmployees();
        }

        private static void LazyLoadLocationEmployees()
        {
            using (var context = new CoffeeShopContext())
            {
                var locations = context.Locations.ToList();
                foreach (var l in locations)
                {
                    Console.WriteLine($"{l.StreetAddress}, Hours {l.Hours}");
                    if (l.OpenTime <= TimeSpan.FromHours(7))
                    {
                        foreach (var employee in l.Employees)
                        {
                            Console.WriteLine($"{employee.Name} {(employee.Barista ? "(Barista)" : "")}");
                        }
                    }
                }
                Console.ReadKey();
            }
        }
    }
}