using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Specifikacije
    {
        public Specifikacije()
        {
            Modeli = new HashSet<Modeli>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru specifikacije")]
        public int IdSpecifikacija { get; set; }
        public int? KonjskeSnage { get; set; }
        public int? IdMjenjaca { get; set; }
        public int? IdBoje { get; set; }
        public int? IdDodatneOpreme { get; set; }
        public int? IdVrsteGoriva { get; set; }
        public int? VelicinaTankaULitrima { get; set; }
        public float? Potrosnja { get; set; }

        public virtual Boje IdBojeNavigation { get; set; }
        public virtual DodatnaOprema IdDodatneOpremeNavigation { get; set; }
        public virtual Mjenjaci IdMjenjacaNavigation { get; set; }
        public virtual VrsteGoriva IdVrsteGorivaNavigation { get; set; }
        public virtual ICollection<Modeli> Modeli { get; set; }
    }
}
