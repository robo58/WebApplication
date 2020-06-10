using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class VozilosViewModel
    {
        public int IdVozila { get; set; }
        public string NazivProizvodjaca { get; set; }
        public ModelViewModel Model { get; set; }
        public int Cijena { get; set; }
        public bool Dostupno { get; set; }

    }
}