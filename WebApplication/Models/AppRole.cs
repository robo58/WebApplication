using Microsoft.AspNetCore.Identity;

namespace WebApplication.Models
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole(){}
        public AppRole(string roleName) : base(roleName)
        {
            
        }
    }
}