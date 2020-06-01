using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class ZaposleniciCertifikati
    {
        public int IdZc { get; set; }
        public int? IdZaposlenika { get; set; }
        public int? IdCertifikata { get; set; }

        public virtual Certifikati IdCertifikataNavigation { get; set; }
        public virtual Zaposlenici IdZaposlenikaNavigation { get; set; }
    }
}
