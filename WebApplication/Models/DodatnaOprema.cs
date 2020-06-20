using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Models
{
    public partial class DodatnaOprema
    {
        public DodatnaOprema()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru Opreme")]
        [Remote(action: nameof(DodatnaOpremaController.ProvjeriSifruArtikla), controller: "Artikl", 
            ErrorMessage = "Oprema s navedenom sifrom vec postoji")]
        public int IdDodatneOpreme { get; set; }
        public bool Siber { get; set; }
        public bool Klima { get; set; }
        public bool KozniSicevi { get; set; }

        public override string ToString()
        {
            string s = "";
            if (Klima)
            {
                s += "Klima,";
            }

            if (Siber)
            {
                s += "Siber,";
            }

            if (KozniSicevi)
            {
                s += "Kozni Sicevi,";
            }

            return s;
        }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
