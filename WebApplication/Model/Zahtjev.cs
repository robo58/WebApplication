using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Zahtjev
    {
        public Zahtjev()
        {
            Ponuda = new HashSet<Ponuda>();
        }

        public int IdZahtjeva { get; set; }
        public int? IdKlijenta { get; set; }
        public int? IdUsluge { get; set; }
        public int? RutaKilometri { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public int? BrojVozila { get; set; }

        public virtual Klijenti IdKlijentaNavigation { get; set; }
        public virtual Usluge IdUslugeNavigation { get; set; }
        public virtual ICollection<Ponuda> Ponuda { get; set; }
    }
}
