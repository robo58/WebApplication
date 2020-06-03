using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
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
                return View(specifikacija);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Specifikacije specifikacija = await _ctx.Specifikacije.FindAsync(id);
                if (specifikacija == null)
                {
                    return NotFound($"Ne postoji specifikacija s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Specifikacije>(specifikacija,
                    "", b=>b.IdSpecifikacija, b=>b.KonjskeSnage, b=>b.IdMjenjaca,
                    b=>b.IdBoje, b=>b.IdDodatneOpreme, b=>b.IdVrsteGoriva, b=>b.VelicinaTankaULitrima,b=>b.Potrosnja);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"specifikacija uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(specifikacija);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o specifikacija nije moguce povezati.");
                    return View(specifikacija);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja specifikacija." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
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