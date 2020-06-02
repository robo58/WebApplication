using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class CertifikatiViewModel
    {
        public IEnumerable<Certifikati> Certifikati { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}