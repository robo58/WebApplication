using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ZahtjevViewModel
    {
        public int IdZahtjeva { get; set; }
        public string ImePrezime { get; set; }
        public string NazivUsluge { get; set; }
        public int? RutaKilometri { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public int? BrojVozila { get; set; }

    }
}