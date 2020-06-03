using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class MjenjaciViewModel
    {
        public IEnumerable<Mjenjaci> Mjenjaci { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}