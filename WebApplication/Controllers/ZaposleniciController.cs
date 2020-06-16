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
    public class ZaposleniciController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public ZaposleniciController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Zaposlenici.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Zaposlenici, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdZaposlenika;
                    break;
                case 2:
                    orderSelector = b => b.IdOdjelaNavigation.Naziv;
                    break;
                case 3:
                    orderSelector = b => b.IdOsobeNavigation.FirstName;
                    break;
                case 4:
                    orderSelector = b => b.RadniStaz;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var zaposlenici =query
                .Select(s=>new ZaposlenikViewModel
                {
                    IdZaposlenika = s.IdZaposlenika,
                    NazivOdjela = s.IdOdjelaNavigation.Naziv,
                    ImeOsobe = s.IdOsobeNavigation.FirstName + " " + s.IdOsobeNavigation.LastName,
                    RadniStaz = s.RadniStaz
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new ZaposleniciViewModel
            {
                Zaposlenici = zaposlenici,
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
            var odjeli = _ctx.Odjeli.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdOdjela}).ToList();
            var osobe = _ctx.AspNetUsers.AsNoTracking().OrderBy(d => d.FirstName)
                .Select(d => new {ImePrezime = (d.FirstName + " " + d.LastName), d.Id}).ToList();

            ViewBag.Odjeli = new SelectList(odjeli, nameof(Odjeli.IdOdjela), nameof(Odjeli.Naziv));
            ViewBag.Osobe = new SelectList(osobe, nameof(AspNetUsers.Id), "ImePrezime");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Zaposlenici zaposlenik)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(zaposlenik);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"zaposlenik uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(zaposlenik);
                }
            }
            else
            {
                return View(zaposlenik);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zaposlenik = _ctx.Zaposlenici
                            .AsNoTracking()
                            .Where(b => b.IdZaposlenika == id)
                            .FirstOrDefault();
            if (zaposlenik == null)
            {
                return NotFound($"Ne postoji zaposlenik s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(zaposlenik);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Zaposlenici zaposlenik, int page = 1, int sort = 1, bool ascending = true)
        {
            if (zaposlenik == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Zaposlenici.Any(m => m.IdZaposlenika == zaposlenik.IdZaposlenika);
            if (!checkId)
            {
                return NotFound($"Neispravan id zaposlenika: {zaposlenik?.IdZaposlenika}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(zaposlenik);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "zaposlenik ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(zaposlenik);
                }
            }
            else
            {
                return View(zaposlenik);
            }
        }

        public IActionResult Row(int id)
        {
            var zaposlenik = _ctx.Zaposlenici                       
                .Where(s => s.IdZaposlenika == id)
                .Select(s=>new ZaposlenikViewModel
                {
                    IdZaposlenika = s.IdZaposlenika,
                    NazivOdjela = s.IdOdjelaNavigation.Naziv,
                    ImeOsobe = s.IdOsobeNavigation.FirstName + " " + s.IdOsobeNavigation.LastName,
                    RadniStaz = s.RadniStaz
                })
                .SingleOrDefault();
            if (zaposlenik != null)
            {
                return PartialView(zaposlenik);
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
            var zaposlenik = await _ctx.Zaposlenici.FindAsync(id);
            if (zaposlenik == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(zaposlenik);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"zaposlenik sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja zaposlenika {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}