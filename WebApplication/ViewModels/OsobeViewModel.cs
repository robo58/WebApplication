using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class OsobeViewModel
    {
        public IEnumerable<Osobe> Osobe { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}