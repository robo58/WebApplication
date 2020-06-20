using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using PdfRpt.FluentInterface;
using PdfRpt.Core.Contracts;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ZahtjevController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;
        private UserManager<AppUser> UserMgr;
        private readonly IHostingEnvironment _hostingEnv;

        public ZahtjevController(IHostingEnvironment hosting,PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot, UserManager<AppUser> userManager)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
            UserMgr = userManager;
            _hostingEnv = hosting;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Zahtjev.AsNoTracking();
            int count = query.Count();
            
            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Ascending = ascending,
                Sort = sort,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            
            if (page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = pagingInfo.TotalPages, sort, ascending });
            }

            System.Linq.Expressions.Expression<Func<Zahtjev, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdZahtjeva;
                    break;
                case 2:
                    orderSelector = b => b.IdUslugeNavigation.NazivUsluge;
                    break;
                case 3:
                    orderSelector = b => b.IdKlijenta;
                    break;
                case 4:
                    orderSelector = b => b.RutaKilometri;
                    break;
                case 5:
                    orderSelector = b => b.DatumOd;
                    break;
                case 6:
                    orderSelector = b => b.DatumDo;
                    break;
                case 7:
                    orderSelector = b => b.BrojVozila;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var zahtjevi =query
                .Select(s=>new ZahtjevViewModel
                {
                    IdZahtjeva = s.IdZahtjeva,
                    NazivUsluge = s.IdUslugeNavigation.NazivUsluge,
                    ImePrezime = s.IdKlijentaNavigation.FirstName + " " + s.IdKlijentaNavigation.LastName,
                    DatumOd = s.DatumOd,
                    DatumDo = s.DatumDo,
                    BrojVozila = s.BrojVozila
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new ZahtjeviViewModel
            {
                Zahtjevi = zahtjevi,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }

        private void PrepareDropDownLists()
        {
            var usluge = _ctx.Usluge.AsNoTracking().OrderBy(d => d.NazivUsluge)
                .Select(d => new {d.NazivUsluge, d.IdUsluge}).ToList();
            var klijenti = _ctx.Users.AsNoTracking().OrderBy(d => d.Id)
                .Select(d => new {ImePrezime=d.FirstName + " " + d.LastName, d.Id}).ToList();

            ViewBag.Usluge = new SelectList(usluge, nameof(Usluge.IdUsluge), nameof(Usluge.NazivUsluge));
            ViewBag.Klijenti = new SelectList(klijenti, nameof(AppUser.Id), "ImePrezime");
        }

        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = _ctx.Zahtjev
                            .AsNoTracking()
                            .Where(b => b.IdZahtjeva == id)
                            .FirstOrDefault();
            if (zahtjev == null)
            {
                return NotFound($"Ne postoji zahtjev s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(zahtjev);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Zahtjev zahtjev, int page = 1, int sort = 1, bool ascending = true)
        {
            if (zahtjev == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Zahtjev.Any(m => m.IdZahtjeva == zahtjev.IdZahtjeva);
            if (!checkId)
            {
                return NotFound($"Neispravan id zahtjeva: {zahtjev?.IdZahtjeva}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(zahtjev);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "zahtjev ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(zahtjev);
                }
            }
            else
            {
                return View(zahtjev);
            }
        }

        public IActionResult Row(int id)
        {
            var zahtjev = _ctx.Zahtjev                       
                .Where(s => s.IdZahtjeva == id)
                .Select(s=>new ZahtjevViewModel
                {
                    IdZahtjeva = s.IdZahtjeva,
                    NazivUsluge = s.IdUslugeNavigation.NazivUsluge,
                    ImePrezime = s.IdKlijentaNavigation.FirstName + " " +s.IdKlijentaNavigation.LastName,
                    DatumOd = s.DatumOd,
                    DatumDo = s.DatumDo,
                    BrojVozila = s.BrojVozila
                })
                .SingleOrDefault();
            if (zahtjev != null)
            {
                return PartialView(zahtjev);
            }
            else
            {
                //vratiti prazan sadržaj?
                return NoContent();
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = await _ctx.Zahtjev.FindAsync(id);
            if (zahtjev == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(zahtjev);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"zahtjev sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja zahtjeva {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var zahtjev = await _ctx.Zahtjev.FindAsync(id);
            var id_vozila = await _ctx.ZahtjevVozila.Where(d => d.IdZahtjeva == id).Select(d=>d.IdVozila).ToListAsync();
            var vozila = await _ctx.Vozila.Where(v => id_vozila.Contains(v.IdVozila))
                .Select(v => new VozilosViewModel
                {
                    IdVozila = v.IdVozila,
                    Cijena = v.Cijena,
                    Dostupno = v.Dostupno,
                    Model = new ModelViewModel
                    {
                        IdModela = v.IdModelaNavigation.IdModela,
                        Naziv = v.IdModelaNavigation.Naziv,
                        Specifikacija = new SpecifikacijaViewModel
                        {
                            IdSpecifikacija = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdSpecifikacija,
                            KonjskeSnage = v.IdModelaNavigation.IdSpecifikacijaNavigation.KonjskeSnage,
                            NazivBoje = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdBojeNavigation.Naziv,
                            NazivDodatneOpreme = v.IdModelaNavigation.IdSpecifikacijaNavigation
                                .IdDodatneOpremeNavigation.ToString(),
                            NazivMjenjaca = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdMjenjacaNavigation.Naziv,
                            NazivVrsteGoriva = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdVrsteGorivaNavigation
                                .Naziv,
                            Potrosnja = v.IdModelaNavigation.IdSpecifikacijaNavigation.Potrosnja,
                            VelicinaTankaULitrima = v.IdModelaNavigation.IdSpecifikacijaNavigation.VelicinaTankaULitrima
                        }
                    },
                    NazivProizvodjaca = v.IdProizvodjacaNavigation.Naziv,
                    IdSlike = v.IdSlike
                }).ToListAsync();
            var klijent = await _ctx.Users.FindAsync(zahtjev.IdKlijenta);
            var usluga = await _ctx.Usluge.FindAsync(zahtjev.IdUsluge);
            var kategorija = await _ctx.Usluge.Where(d => d.IdUsluge == zahtjev.IdUsluge)
                .Select(d => d.IdKategorijeNavigation.Naziv).FirstOrDefaultAsync();
            
            ViewBag.Vozila = vozila;
            ViewBag.Klijent = klijent;
            ViewBag.Kat = kategorija;
            ViewBag.Usluga = usluga;

            return View(zahtjev);
        }

        [HttpGet]
        public async Task<IActionResult> ShowForm(int IdUsluge,int[] idVozila)
        {
            var usluga = await _ctx.Usluge.FindAsync(IdUsluge);
            var kat = usluga.IdKategorije;
            
            if (kat != null && kat == 2)
            {
                var vozaci = await _ctx.Zaposlenici.AsNoTracking().OrderBy(d=>d.IdOsobe)
                    .Select(d=>new {ImePrezime = d.IdOsobeNavigation.FirstName + " " +d.IdOsobeNavigation.LastName + "Radni staz: " + d.RadniStaz, d.IdZaposlenika}).ToListAsync();
                ViewBag.Vozaci = new SelectList(vozaci,nameof(Zaposlenici.IdZaposlenika),"ImePrezime");
            }
        
            ViewBag.IdUsluge = IdUsluge;
            ViewBag.IdVozila = idVozila;
            return View();
        }

        public void DodajVozila(int idZahtjeva, int[] idVozila)
        {
            foreach (var id in idVozila)
            {
                ZahtjevVozila zahtjevVozila = new ZahtjevVozila();
                zahtjevVozila.IdVozila = id;
                zahtjevVozila.IdZahtjeva = idZahtjeva;
                _ctx.Add(zahtjevVozila);
            }
        }

        public void DodajVozace(int idZahtjeva, int[] idVozaca)
        {
            foreach (var id in idVozaca)
            {
                ZahtjevVozaci zahtjevVozaci = new ZahtjevVozaci();
                zahtjevVozaci.IdVozaca = id;
                zahtjevVozaci.IdZahtjeva = idZahtjeva;
                _ctx.Add(zahtjevVozaci);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(int IdUsluge,int[] idVozila,int[] idVozaca,ClaimsPrincipal user, DateTime? DatumOd, DateTime? DatumDo)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var klijent = await UserMgr.FindByNameAsync(username);
            
            Zahtjev zahtjev = new Zahtjev();
            zahtjev.IdZahtjeva = await _ctx.Zahtjev.CountAsync() + 1;
            zahtjev.IdUsluge = IdUsluge;
            zahtjev.IdKlijenta = klijent.Id;
            zahtjev.DatumOd = DatumOd;
            zahtjev.DatumDo = DatumDo;
            zahtjev.BrojVozila = idVozila.Length;
            DodajVozila(zahtjev.IdZahtjeva,idVozila);
            DodajVozace(zahtjev.IdZahtjeva,idVozaca);
            try
            {
                _ctx.Add(zahtjev);
                _ctx.SaveChanges();
                TempData[Constants.Message] = $"zahtjev uspjesno dodano.*";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                PrepareDropDownLists();
                return RedirectToAction(nameof(ShowForm),new {IdUsluge, idVozila});
            }
        }
    }
}