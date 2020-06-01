using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Kategorije
    {
        public Kategorije()
        {
            Usluge = new HashSet<Usluge>();
        }

        public int IdKategorije { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Usluge> Usluge { get; set; }
    }
}
