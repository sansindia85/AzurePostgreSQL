using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Using object initializer
        //One to many relationships
        //Trying to avoid quotes to null list
        public List<Quote> Quotes { get; set; } = new();

        //Many to Many relationship
        public List<Battle> Battles { get; set; } = new();

        public Horse Horse { get; set; }
    }
}
