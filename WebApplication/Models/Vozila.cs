using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Vozila
    {
        public Vozila()
        {
            ZahtjevVozila = new HashSet<ZahtjevVozila>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru vozila")]
        public int IdVozila { get; set; }
        
        [Required(ErrorMessage = "Potrebno je odabrati proizvodjaca")]
        public int? IdProizvodjaca { get; set; }
        
        [Required(ErrorMessage = "Potrebno je odabrati model")]
        public int? IdModela { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti cijenu vozila")]
        [Range(0, double.MaxValue, ErrorMessage = "Cijena vozila mora biti pozitivan broj")]
        public int Cijena { get; set; }
        public bool Dostupno { get; set; }
        public int? IdSlike { get; set; }

        public virtual Modeli IdModelaNavigation { get; set; }
        public virtual Proizvodjaci IdProizvodjacaNavigation { get; set; }
        public virtual Slike IdSlikeNavigation { get; set; }
        public virtual ICollection<ZahtjevVozila> ZahtjevVozila { get; set; }
    }
}
