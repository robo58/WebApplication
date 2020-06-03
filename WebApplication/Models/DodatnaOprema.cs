using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class DodatnaOprema
    {
        public DodatnaOprema()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        public int IdDodatneOpreme { get; set; }
        public bool Siber { get; set; }
        public bool Klima { get; set; }
        public bool KozniSicevi { get; set; }

        public override string ToString()
        {
            string s = "";
            if (Klima)
            {
                s += "Klima,";
            }

            if (Siber)
            {
                s += "Siber,";
            }

            if (KozniSicevi)
            {
                s += "Kozni Sicevi,";
            }

            return s;
        }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
