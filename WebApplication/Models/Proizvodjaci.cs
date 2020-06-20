using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Proizvodjaci
    {
        public Proizvodjaci()
        {
            Vozila = new HashSet<Vozila>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru proizvodjaca")]
        public int IdProizvodjaca { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv proizvodjaca")]
        public string Naziv { get; set; }

        public virtual ICollection<Vozila> Vozila { get; set; }
    }
}
