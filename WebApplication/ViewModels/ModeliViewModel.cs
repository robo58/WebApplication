using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ModeliViewModel
    {
        public IEnumerable<ModelViewModel> Modeli { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}