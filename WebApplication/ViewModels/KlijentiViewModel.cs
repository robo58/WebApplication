using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class KlijentiViewModel
    {
        public IEnumerable<KlijentViewModel> Klijenti { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}