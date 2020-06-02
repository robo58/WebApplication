using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class BojeViewModel
    {
        public IEnumerable<Boje> Boje { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}