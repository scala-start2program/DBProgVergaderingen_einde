using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Scala.Vergaderingen.Core.Entities
{
    [Table("Personen")]
    public class Persoon
    {
        [ExplicitKey]
        public string Id { get; private set; }
        public string Naam { get; set; }
        public Persoon()
        {
            Id = Guid.NewGuid().ToString();
        }
        public Persoon(string naam)
        {
            Id = Guid.NewGuid().ToString();
            Naam = naam;
        }
        internal Persoon(string id, string naam)
        {
            Id = id;
            Naam = naam;
        }
        public override string ToString()
        {
            return Naam;
        }
    }
}
