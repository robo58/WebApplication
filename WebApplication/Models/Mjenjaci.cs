using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Mjenjaci
    {
        public Mjenjaci()
        {
            Specifikacije = new HashSet<Specifikacije>();
        }

        public int IdMjenjaca { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Specifikacije> Specifikacije { get; set; }
    }
}
