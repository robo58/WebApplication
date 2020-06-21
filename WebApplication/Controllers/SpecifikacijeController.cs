using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "admin,zaposlenik")]
    public class SpecifikacijeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public SpecifikacijeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Specifikacije.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Specifikacije, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdSpecifikacija;
                    break;
                case 2:
                    orderSelector = b => b.KonjskeSnage;
                    break;
                case 3:
                    orderSelector = b => b.IdMjenjacaNavigation.Naziv;
                    break;
                case 4:
                    orderSelector = b => b.IdBojeNavigation.Naziv;
                    break;
                case 5:
                    orderSelector = b => b.IdDodatneOpremeNavigation.ToString();
                    break;
                case 6:
                    orderSelector = b => b.IdVrsteGorivaNavigation.Naziv;
                    break;
                case 7:
                    orderSelector = b => b.VelicinaTankaULitrima;
                    break;
                case 8:
                    orderSelector = b => b.Potrosnja;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var specifikacije =query
                .Select(s=>new SpecifikacijaViewModel
                {
                    IdSpecifikacija = s.IdSpecifikacija,
                    KonjskeSnage = s.KonjskeSnage,
                    NazivMjenjaca = s.IdMjenjacaNavigation.Naziv,
                    NazivBoje = s.IdBojeNavigation.Naziv,
                    NazivDodatneOpreme = s.IdDodatneOpremeNavigation.ToString(),
                    NazivVrsteGoriva = s.IdVrsteGorivaNavigation.Naziv,
                    VelicinaTankaULitrima = s.VelicinaTankaULitrima,
                    Potrosnja = s.Potrosnja
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new SpecifikacijeViewModel()
            {
                Specifikacije = specifikacije,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            PrepareDropDownLists();
            return View();
        }
        
        private void PrepareDropDownLists()
        {
            var mjenjaci = _ctx.Mjenjaci.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdMjenjaca}).ToList();
            var boje = _ctx.Boje.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdBoje}).ToList();
            var dodatnaOprema = _ctx.DodatnaOprema.AsNoTracking().OrderBy(d=>d.IdDodatneOpreme)
                .Select(d=>new OpremaHelper{IdOpreme = d.IdDodatneOpreme, NazivOpreme = d.ToString()}).ToList();
            var vrsteGoriva = _ctx.VrsteGoriva.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdVrsteGoriva}).ToList();
            ViewBag.Mjenjaci = new SelectList(mjenjaci, nameof(Mjenjaci.IdMjenjaca), nameof(Mjenjaci.Naziv));
            ViewBag.Boje = new SelectList(boje, nameof(Boje.IdBoje), nameof(Boje.Naziv));
            ViewBag.DodatnaOprema = new SelectList(dodatnaOprema, nameof(OpremaHelper.IdOpreme), nameof(OpremaHelper.NazivOpreme));
            ViewBag.VrsteGoriva = new SelectList(vrsteGoriva, nameof(VrsteGoriva.IdVrsteGoriva), nameof(VrsteGoriva.Naziv));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Specifikacije specifikacija)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(specifikacija);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"specifikacija uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(specifikacija);
                }
            }
            else
            {
                return View(specifikacija);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var specifikacija = _ctx.Specifikacije
                            .AsNoTracking()
                            .Where(b => b.IdSpecifikacija == id)
                            .FirstOrDefault();
            if (specifikacija == null)
            {
                return NotFound($"Ne postoji specifikacija s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(specifikacija);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Specifikacije specifikacija, int page = 1, int sort = 1, bool ascending = true)
        {
            if (specifikacija == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Specifikacije.Any(m => m.IdSpecifikacija == specifikacija.IdSpecifikacija);
            if (!checkId)
            {
                return NotFound($"Neispravan id specifikacije: {specifikacija?.IdSpecifikacija}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(specifikacija);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "Specifikacija ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(specifikacija);
                }
            }
            else
            {
                return View(specifikacija);
            }
        }

        public IActionResult Row(int id)
        {
            var specifikacija = _ctx.Specifikacije                       
                .Where(s => s.IdSpecifikacija == id)
                .Select(s => new SpecifikacijaViewModel
                {
                    IdSpecifikacija = s.IdSpecifikacija,
                    KonjskeSnage = s.KonjskeSnage,
                    NazivMjenjaca = s.IdMjenjacaNavigation.Naziv,
                    NazivBoje = s.IdBojeNavigation.Naziv,
                    NazivDodatneOpreme = s.IdDodatneOpremeNavigation.ToString(),
                    NazivVrsteGoriva = s.IdVrsteGorivaNavigation.Naziv,
                    VelicinaTankaULitrima = s.VelicinaTankaULitrima,
                    Potrosnja = s.Potrosnja
                })
                .SingleOrDefault();
            if (specifikacija != null)
            {
                return PartialView(specifikacija);
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
            var specifikacija = await _ctx.Specifikacije.FindAsync(id);
            if (specifikacija == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(specifikacija);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"Specifikacija sa sifrom {id} obrisano.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja specifikacije {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}