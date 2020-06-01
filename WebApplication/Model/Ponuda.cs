using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Ponuda
    {
        public Ponuda()
        {
            PonudaVozac = new HashSet<PonudaVozac>();
            PonudaVozilo = new HashSet<PonudaVozilo>();
        }

        public int IdPonude { get; set; }
        public int? IdZahtjeva { get; set; }
        public int? PopustKolicinaPostotak { get; set; }
        public int? DodatniPopustPostotak { get; set; }

        public virtual Zahtjev IdZahtjevaNavigation { get; set; }
        public virtual ICollection<PonudaVozac> PonudaVozac { get; set; }
        public virtual ICollection<PonudaVozilo> PonudaVozilo { get; set; }
    }
}
