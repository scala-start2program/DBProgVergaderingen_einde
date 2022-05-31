using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Scala.Vergaderingen.Core.Entities
{
    [Table("Deelnemers")]
    public class Deelnemer
    {
        [ExplicitKey]
        public string VergaderingId { get; set; }
        [ExplicitKey]
        public string PersoonId { get; set; }
        public Deelnemer(string vergaderingId, string persoonId)
        {
            VergaderingId = vergaderingId;
            PersoonId = persoonId;
        }
    }
}
