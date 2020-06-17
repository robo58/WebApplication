using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<AppUser> SignInMgr;
        private UserManager<AppUser> UserMgr;
        
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            SignInMgr = signInManager;
            UserMgr = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username,string password,string email, string firstname, string lastname)
        {
            try
            {
                ViewBag.Message = "User already registered!";

                AppUser user = await UserMgr.FindByNameAsync(username);
                if (user == null)
                {
                    user = new AppUser();
                    user.UserName = username;
                    user.Email = email;
                    user.FirstName = firstname;
                    user.LastName = lastname;
                    IdentityResult result = await UserMgr.CreateAsync(user, password);
                    IdentityResult result2 = await UserMgr.AddToRoleAsync(user, "user");
                    ViewBag.Message = "User created!";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username,string password)
        {
            var result = await SignInMgr.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Result = "result is: " + result.ToString();
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await SignInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}