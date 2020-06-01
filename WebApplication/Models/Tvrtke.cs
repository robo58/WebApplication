using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Tvrtke
    {
        public Tvrtke()
        {
            Klijenti = new HashSet<Klijenti>();
        }

        public int IdTvrtke { get; set; }
        public string Naziv { get; set; }
        public int? DogovoreniPopustPostotak { get; set; }
        public int? VrijemeSuradnjeGodine { get; set; }

        public virtual ICollection<Klijenti> Klijenti { get; set; }
    }
}
