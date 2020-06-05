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
    public class KlijentiController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public KlijentiController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Klijenti.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Klijenti, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdKlijenta;
                    break;
                case 2:
                    orderSelector = b => b.IdOsobeNavigation.Ime;
                    break;
                case 3:
                    orderSelector = b => b.IdTvrtkeNavigation.Naziv;
                    break; ;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var klijenti =query
                .Select(s=>new KlijentViewModel
                {
                    IdKlijenta = s.IdKlijenta,
                    ImePrezime = (s.IdOsobeNavigation.Ime + " " + s.IdOsobeNavigation.Prezime),
                    NazivTvrtke = s.IdTvrtkeNavigation.Naziv
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new KlijentiViewModel()
            {
                Klijenti = klijenti,
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
            var osobe = _ctx.Osobe.AsNoTracking().OrderBy(d => d.Ime)
                .Select(d => new {ImePrezime=(d.Ime + " " + d.Prezime), d.IdOsobe}).ToList();
            var tvrtke = _ctx.Tvrtke.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdTvrtke}).ToList();
            
            ViewBag.Osobe = new SelectList(osobe, nameof(Osobe.IdOsobe), "ImePrezime");
            ViewBag.Tvrtke = new SelectList(tvrtke, nameof(Tvrtke.IdTvrtke), nameof(Tvrtke.Naziv));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Klijenti klijent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(klijent);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"klijent uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(klijent);
                }
            }
            else
            {
                return View(klijent);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var klijent = _ctx.Klijenti
                            .AsNoTracking()
                            .Where(b => b.IdKlijenta == id)
                            .FirstOrDefault();
            if (klijent == null)
            {
                return NotFound($"Ne postoji klijent s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(klijent);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Klijenti klijent, int page = 1, int sort = 1, bool ascending = true)
        {
            if (klijent == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Klijenti.Any(m => m.IdKlijenta == klijent.IdKlijenta);
            if (!checkId)
            {
                return NotFound($"Neispravan id klijenta: {klijent?.IdKlijenta}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(klijent);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "klijent ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(klijent);
                }
            }
            else
            {
                return View(klijent);
            }
        }

        public IActionResult Row(int id)
        {
            var klijent = _ctx.Klijenti                       
                .Where(s => s.IdKlijenta == id)
                .Select(s => new KlijentViewModel
                {
                    IdKlijenta = s.IdKlijenta,
                    ImePrezime = (s.IdOsobeNavigation.Ime + " " + s.IdOsobeNavigation.Prezime),
                    NazivTvrtke = s.IdTvrtkeNavigation.Naziv
                })
                .SingleOrDefault();
            if (klijent != null)
            {
                return PartialView(klijent);
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
            var klijent = await _ctx.Klijenti.FindAsync(id);
            if (klijent == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(klijent);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"klijent sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja klijenta {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}