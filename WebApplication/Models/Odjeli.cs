using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Odjeli
    {
        public Odjeli()
        {
            Zaposlenici = new HashSet<Zaposlenici>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru odjela")]
        public int IdOdjela { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv odjela")]
        public string Naziv { get; set; }

        public virtual ICollection<Zaposlenici> Zaposlenici { get; set; }
    }
}
