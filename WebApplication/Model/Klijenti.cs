using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Klijenti
    {
        public Klijenti()
        {
            Zahtjev = new HashSet<Zahtjev>();
        }

        public int IdKlijenta { get; set; }
        public int? IdOsobe { get; set; }
        public int? IdTvrtke { get; set; }

        public virtual Osobe IdOsobeNavigation { get; set; }
        public virtual Tvrtke IdTvrtkeNavigation { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }
    }
}
