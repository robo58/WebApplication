using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class SpecifikacijeViewModel
    {
        public IEnumerable<SpecifikacijaViewModel> Specifikacije { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}