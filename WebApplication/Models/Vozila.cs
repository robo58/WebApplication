using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Vozila
    {
        public Vozila()
        {
            PonudaVozilo = new HashSet<PonudaVozilo>();
            SlikeVozila = new HashSet<SlikeVozila>();
        }

        public int IdVozila { get; set; }
        public int? IdProizvodjaca { get; set; }
        public int? IdModela { get; set; }
        public int? Cijena { get; set; }
        public bool? Dostupno { get; set; }

        public virtual Modeli IdModelaNavigation { get; set; }
        public virtual Proizvodjaci IdProizvodjacaNavigation { get; set; }
        public virtual ICollection<PonudaVozilo> PonudaVozilo { get; set; }
        public virtual ICollection<SlikeVozila> SlikeVozila { get; set; }
    }
}
