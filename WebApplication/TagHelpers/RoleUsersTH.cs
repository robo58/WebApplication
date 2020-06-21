using WebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "i-role")]  
    public class RoleUsersTH : TagHelper
    {
        private UserManager<AppUser> userManager;
        private RoleManager<AppRole> roleManager;
  
        public RoleUsersTH(UserManager<AppUser> usermgr, RoleManager<AppRole> rolemgr)
        {
            userManager = usermgr;
            roleManager = rolemgr;
        }
  
        [HtmlAttributeName("i-role")]
        public string Role { get; set; }
  
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> names = new List<string>();
            AppRole role = await roleManager.FindByNameAsync(Role);
            if (role != null)
            {
                foreach (var user in userManager.Users)
                {
                    if (user != null && await userManager.IsInRoleAsync(user, role.Name))
                        names.Add(user.UserName);
                }
            }
            output.Content.SetContent(names.Count == 0 ? "No Users" : string.Join(", ", names));
        }
    }
}