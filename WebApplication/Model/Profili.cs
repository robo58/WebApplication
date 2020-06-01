using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Profili
    {
        public int IdProfila { get; set; }
        public int? IdZaposlenika { get; set; }

        public virtual Zaposlenici IdZaposlenikaNavigation { get; set; }
    }
}
