using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class UslugeViewModel
    {
        public IEnumerable<UslugaViewModel> Usluge { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}