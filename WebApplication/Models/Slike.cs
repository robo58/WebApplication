using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Slike
    {
        public Slike()
        {
            Vozila = new HashSet<Vozila>();
        }

        public int IdSlike { get; set; }
        public byte[] SlikaBinary { get; set; }

        public virtual ICollection<Vozila> Vozila { get; set; }
    }
}
