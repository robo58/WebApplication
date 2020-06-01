using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Proizvodjaci
    {
        public Proizvodjaci()
        {
            Vozila = new HashSet<Vozila>();
        }

        public int IdProizvodjaca { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Vozila> Vozila { get; set; }
    }
}
