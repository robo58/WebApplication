using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ModelViewModel
    {
        public int IdModela { get; set; }
        public string Naziv { get; set; }
        public SpecifikacijaViewModel Specifikacija { get; set; }

    }
}