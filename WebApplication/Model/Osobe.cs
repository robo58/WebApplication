using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Osobe
    {
        public Osobe()
        {
            Klijenti = new HashSet<Klijenti>();
            Zaposlenici = new HashSet<Zaposlenici>();
        }

        public int IdOsobe { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime? DatumRodjenja { get; set; }

        public virtual ICollection<Klijenti> Klijenti { get; set; }
        public virtual ICollection<Zaposlenici> Zaposlenici { get; set; }
    }
}
