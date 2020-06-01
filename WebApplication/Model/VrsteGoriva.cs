using System;
using System.Collections.Generic;

namespace WebApplication.Model
{
    public partial class VrsteGoriva
    {
        public VrsteGoriva()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        public int IdVrsteGoriva { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
