using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class VoziloViewModel
    {
        public int IdVozila { get; set; }
        public string NazivProizvodjaca { get; set; }
        public string NazivModela { get; set; }
        public int Cijena { get; set; }
        public bool Dostupno { get; set; }

    }
}