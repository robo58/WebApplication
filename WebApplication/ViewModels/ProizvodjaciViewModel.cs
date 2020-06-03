using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ProizvodjaciViewModel
    {
        public IEnumerable<Proizvodjaci> Proizvodjaci { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}