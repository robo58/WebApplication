using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class VrsteGoriva
    {
        public VrsteGoriva()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru vrste goriva")]
        public int IdVrsteGoriva { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv vrste goriva")]
        public string Naziv { get; set; }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
