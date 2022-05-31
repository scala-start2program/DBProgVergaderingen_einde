using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Scala.Vergaderingen.Core.Entities
{
    [Table("Vergaderingen")]
    public class Vergadering
    {
        [ExplicitKey]
        public string Id { get; private set; }
        public DateTime Datum { get; set; }
        public string Van { get; set; }
        public string Tot { get; set; }
        public string Locatie { get; set; }
        public Vergadering()
        {
            Id = Guid.NewGuid().ToString();
        }
        public Vergadering(DateTime datum, string van, string tot, string locatie)
        {
            Id = Guid.NewGuid().ToString();
            Datum = datum;
            Van = van;
            Tot = tot;
            Locatie = locatie;
        }
        internal Vergadering(string id, DateTime datum, string van, string tot, string locatie)
        {
            Id = id;
            Datum = datum;
            Van = van;
            Tot = tot;
            Locatie = locatie;
        }
        public override string ToString()
        {
            return $"{Datum.ToString("ddd dd/MM/yyy")} {Van}-{Tot}";
        }
    }
}
