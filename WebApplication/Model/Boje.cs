using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class Boje
    {
        public Boje()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        public int IdBoje { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
