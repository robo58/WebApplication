using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class SlikeVozila
    {
        public int IdSv { get; set; }
        public int? IdSlike { get; set; }
        public int? IdVozila { get; set; }

        public virtual Slike IdSlikeNavigation { get; set; }
        public virtual Vozila IdVozilaNavigation { get; set; }
    }
}
