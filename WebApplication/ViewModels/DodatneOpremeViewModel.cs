using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class DodatneOpremeViewModel
    {
        public IEnumerable<DodatnaOprema> Oprema { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}