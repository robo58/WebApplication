using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ProfiliViewModel
    {
        public IEnumerable<ProfilViewModel> Profili { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}