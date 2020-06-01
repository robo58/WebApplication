using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class PonudaVozac
    {
        public int IdPv { get; set; }
        public int? IdPonude { get; set; }
        public int? IdVozaca { get; set; }

        public virtual Ponuda IdPonudeNavigation { get; set; }
        public virtual Zaposlenici IdVozacaNavigation { get; set; }
    }
}
