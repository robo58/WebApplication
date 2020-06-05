using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class TvrtkeViewModel
    {
        public IEnumerable<Tvrtke> Tvrtke { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}