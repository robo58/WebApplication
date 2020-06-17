using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class ZahtjevVozaci
    {
        public int IdZahtjeva { get; set; }
        public int IdVozaca { get; set; }

        public virtual Zaposlenici IdVozacaNavigation { get; set; }
        public virtual Zahtjev IdZahtjevaNavigation { get; set; }
    }
}
