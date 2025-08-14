using EntityFrameworkCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Domain
{
    public  class Team: BaseDomainObject
    {
        public string Name { get; set; }
        // EF Core infiere que esta es una llave foranea de la otra tabla 
        public int LeagueId { get; set; } // llave fornea
        public virtual League League { get; set; }  // propiedad de navegacion
        public virtual Coach Coach { get; set; }
        public virtual List<Match> HomeMatches { get; set; } = [];
        public virtual List<Match> AwayMatches { get; set; } = [];
    }
}
