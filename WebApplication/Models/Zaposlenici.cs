﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Zaposlenici
    {
        public Zaposlenici()
        {
            ZahtjevVozaci = new HashSet<ZahtjevVozaci>();
        }

        [Required(ErrorMessage = "Potrebno je unijeti sifru zaposlenika")]
        public int IdZaposlenika { get; set; }
        public int? IdOdjela { get; set; }
        public int? IdOsobe { get; set; }
        public int? RadniStaz { get; set; }

        public virtual Odjeli IdOdjelaNavigation { get; set; }
        public virtual AppUser IdOsobeNavigation { get; set; }
        public virtual ICollection<ZahtjevVozaci> ZahtjevVozaci { get; set; }
    }
}
