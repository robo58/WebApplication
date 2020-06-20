using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Models
{
    public partial class Mjenjaci
    {
        public Mjenjaci()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru mjenjaca")]
        public int IdMjenjaca { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv mjenjaca")]
        public string Naziv { get; set; }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
