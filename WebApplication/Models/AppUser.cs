using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.Models
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser()
        {
            Zahtjev = new HashSet<Zahtjev>();
            Zaposlenici = new HashSet<Zaposlenici>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }
        public virtual ICollection<Zaposlenici> Zaposlenici { get; set; }
    }
}