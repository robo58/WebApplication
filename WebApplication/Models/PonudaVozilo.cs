using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class PonudaVozilo
    {
        public int IdPvozilo { get; set; }
        public int? IdPonude { get; set; }
        public int? IdVozila { get; set; }

        public virtual Ponuda IdPonudeNavigation { get; set; }
        public virtual Vozila IdVozilaNavigation { get; set; }
    }
}
