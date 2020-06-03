using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class SpecifikacijaViewModel
    {
        public int IdSpecifikacija { get; set; }
        public int? KonjskeSnage { get; set; }
        public string NazivMjenjaca { get; set; }
        public string NazivBoje { get; set; }
        public string NazivDodatneOpreme { get; set; }
        public string NazivVrsteGoriva { get; set; }
        public int? VelicinaTankaULitrima { get; set; }
        public float? Potrosnja { get; set; }
    }
}