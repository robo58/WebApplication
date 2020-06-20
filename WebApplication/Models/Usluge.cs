using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Usluge
    {
        public Usluge()
        {
            Zahtjev = new HashSet<Zahtjev>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru usluge")]
        public int IdUsluge { get; set; }
        
        [Required(ErrorMessage = "Potrebno je odabrati kategoriju")]
        public int? IdKategorije { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv usluge")]
        public string NazivUsluge { get; set; }

        public virtual Kategorije IdKategorijeNavigation { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }
    }
}
