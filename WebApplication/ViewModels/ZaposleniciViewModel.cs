using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ZaposleniciViewModel
    {
        public IEnumerable<ZaposlenikViewModel> Zaposlenici { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}