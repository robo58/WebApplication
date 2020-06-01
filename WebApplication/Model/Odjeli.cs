using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Odjeli
    {
        public Odjeli()
        {
            Zaposlenici = new HashSet<Zaposlenici>();
        }

        public int IdOdjela { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Zaposlenici> Zaposlenici { get; set; }
    }
}
