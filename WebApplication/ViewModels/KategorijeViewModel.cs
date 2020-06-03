using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class KategorijeViewModel
    {
        public IEnumerable<Kategorije> Kategorije { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}