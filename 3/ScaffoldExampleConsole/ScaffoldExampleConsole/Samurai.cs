using System;
using System.Collections.Generic;

#nullable disable

namespace ScaffoldExampleConsole
{
    public partial class Samurai
    {
        public Samurai()
        {
            Quotes = new HashSet<Quote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }
    }
}
