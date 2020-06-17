using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class ZahtjevVozila
    {
        public int IdZahtjeva { get; set; }
        public int IdVozila { get; set; }

        public virtual Vozila IdVozilaNavigation { get; set; }
        public virtual Zahtjev IdZahtjevaNavigation { get; set; }
    }
}
