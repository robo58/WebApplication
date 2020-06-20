using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Models
{
    public partial class Modeli
    {
        public Modeli()
        {
            Vozila = new HashSet<Vozila>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru modela")]
        public int IdModela { get; set; }
        
        [Required(ErrorMessage = "Potrebno je unijeti naziv modela")]
        public string Naziv { get; set; }
        
        [Required(ErrorMessage = "Potrebno je odabrati specifikacije")]
        public int? IdSpecifikacija { get; set; }
        
        [Required(ErrorMessage = "Potrebno je odabrati tip")]
        public string Tip { get; set; }

        public virtual Specifikacije IdSpecifikacijaNavigation { get; set; }
        public virtual ICollection<Vozila> Vozila { get; set; }
    }
}
