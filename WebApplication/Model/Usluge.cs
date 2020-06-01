using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Usluge
    {
        public Usluge()
        {
            Zahtjev = new HashSet<Zahtjev>();
        }

        public int IdUsluge { get; set; }
        public int? IdKategorije { get; set; }
        public string NazivUsluge { get; set; }

        public virtual Kategorije IdKategorijeNavigation { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }
    }
}
