using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class ProfileController : Controller
    {
        private SignInManager<AppUser> SignInMgr;
        private UserManager<AppUser> UserMgr;
        private readonly PI10Context _ctx;
        
        public ProfileController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager,PI10Context ctx)
        {
            SignInMgr = signInManager;
            UserMgr = userManager;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var user = await _ctx.Users.FindAsync(id);
            var zahtjevi = await _ctx.Zahtjev.Where(d => d.IdKlijenta == id).Select(d => new ZahtjevViewModel
            {
                IdZahtjeva = d.IdZahtjeva,
                BrojVozila = d.BrojVozila,
                DatumDo = d.DatumDo,
                DatumOd = d.DatumOd,
                ImePrezime = user.FirstName + " " + user.LastName,
                NazivUsluge = d.IdUslugeNavigation.NazivUsluge,
            }).ToListAsync();
            if (user != null)
            {
                ViewBag.Zahtjevi = zahtjevi;
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }
        
        

    }
}