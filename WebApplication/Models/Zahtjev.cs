using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Zahtjev
    {
        public Zahtjev()
        {
            ZahtjevVozaci = new HashSet<ZahtjevVozaci>();
            ZahtjevVozila = new HashSet<ZahtjevVozila>();
        }

        public int IdZahtjeva { get; set; }
        public int? IdKlijenta { get; set; }
        public int? IdUsluge { get; set; }
        public int? RutaKilometri { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public int? BrojVozila { get; set; }

        public virtual AppUser IdKlijentaNavigation { get; set; }
        public virtual Usluge IdUslugeNavigation { get; set; }
        public virtual ICollection<ZahtjevVozaci> ZahtjevVozaci { get; set; }
        public virtual ICollection<ZahtjevVozila> ZahtjevVozila { get; set; }
    }
}
