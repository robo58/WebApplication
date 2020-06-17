using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Zaposlenici
    {
        public Zaposlenici()
        {
            ZahtjevVozaci = new HashSet<ZahtjevVozaci>();
        }

        public int IdZaposlenika { get; set; }
        public int? IdOdjela { get; set; }
        public int? IdOsobe { get; set; }
        public int? RadniStaz { get; set; }

        public virtual Odjeli IdOdjelaNavigation { get; set; }
        public virtual AppUser IdOsobeNavigation { get; set; }
        public virtual ICollection<ZahtjevVozaci> ZahtjevVozaci { get; set; }
    }
}
