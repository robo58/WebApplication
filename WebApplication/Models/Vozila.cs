using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Vozila
    {
        public Vozila()
        {
            ZahtjevVozila = new HashSet<ZahtjevVozila>();
        }

        public int IdVozila { get; set; }
        public int? IdProizvodjaca { get; set; }
        public int? IdModela { get; set; }
        public int Cijena { get; set; }
        public bool Dostupno { get; set; }
        public int? IdSlike { get; set; }

        public virtual Modeli IdModelaNavigation { get; set; }
        public virtual Proizvodjaci IdProizvodjacaNavigation { get; set; }
        public virtual Slike IdSlikeNavigation { get; set; }
        public virtual ICollection<ZahtjevVozila> ZahtjevVozila { get; set; }
    }
}
