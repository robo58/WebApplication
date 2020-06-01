using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Profili
    {
        public int IdProfila { get; set; }
        public int? IdZaposlenika { get; set; }

        public virtual Zaposlenici IdZaposlenikaNavigation { get; set; }
    }
}
