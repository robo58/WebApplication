using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Certifikati
    {
        public Certifikati()
        {
            ZaposleniciCertifikati = new HashSet<ZaposleniciCertifikati>();
        }

        public int IdCertifikata { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<ZaposleniciCertifikati> ZaposleniciCertifikati { get; set; }
    }
}
