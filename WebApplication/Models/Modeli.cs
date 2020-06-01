using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Modeli
    {
        public Modeli()
        {
            Vozila = new HashSet<Vozila>();
        }

        public int IdModela { get; set; }
        public string Naziv { get; set; }
        public int? IdSpecifikacija { get; set; }

        public virtual Specifikacije IdSpecifikacijaNavigation { get; set; }
        public virtual ICollection<Vozila> Vozila { get; set; }
    }
}
