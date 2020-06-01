using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Slike
    {
        public Slike()
        {
            SlikeVozila = new HashSet<SlikeVozila>();
        }

        public int IdSlike { get; set; }
        public string Lokacija { get; set; }

        public virtual ICollection<SlikeVozila> SlikeVozila { get; set; }
    }
}
