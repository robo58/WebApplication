using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class ProfiliController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public ProfiliController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Profili.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Profili, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdProfila;
                    break;
                case 2:
                    orderSelector = b => b.IdZaposlenikaNavigation.IdOsobeNavigation.Ime;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var profili =query
                .Select(s=>new ProfilViewModel
                {
                    IdProfila = s.IdProfila,
                    ImeZaposlenika = s.IdZaposlenikaNavigation.IdOsobeNavigation.Ime + " " + s.IdZaposlenikaNavigation.IdOsobeNavigation.Prezime
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new ProfiliViewModel
            {
                Profili = profili,
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
            var zaposlenici = _ctx.Zaposlenici.AsNoTracking().OrderBy(d => d.IdOsobeNavigation.Ime)
                .Select(d => new {ImePrezime = d.IdOsobeNavigation.Ime + " " + d.IdOsobeNavigation.Prezime, d.IdZaposlenika}).ToList();
            
            ViewBag.Zaposlenici = new SelectList(zaposlenici, nameof(Zaposlenici.IdZaposlenika), "ImePrezime");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Profili profil)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    _ctx.Add(profil);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"profil uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(profil);
                }
            }
            else
            {
                return View(profil);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var profil = _ctx.Profili
                            .AsNoTracking()
                            .Where(b => b.IdZaposlenika == id)
                            .FirstOrDefault();
            if (profil == null)
            {
                return NotFound($"Ne postoji profil s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(profil);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Profili profil,int page = 1, int sort = 1, bool ascending = true)
        {
            if (profil == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await _ctx.Profili.AnyAsync(m => m.IdProfila == profil.IdProfila);
            if (!checkId)
            {
                return NotFound($"Neispravan id profila: {profil?.IdProfila}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(profil);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "profil ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(profil);
                }
            }
            else
            {
                return View(profil);
            }
        }

        public IActionResult Row(int id)
        {
            var profil = _ctx.Profili                       
                .Where(s => s.IdProfila == id)
                .Select(s=>new ProfilViewModel
                {
                    IdProfila = s.IdProfila,
                    ImeZaposlenika = s.IdZaposlenikaNavigation.IdOsobeNavigation.Ime + " " + s.IdZaposlenikaNavigation.IdOsobeNavigation.Prezime,
                })
                .SingleOrDefault();
            if (profil != null)
            {
                return PartialView(profil);
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
            var profil = await _ctx.Profili.FindAsync(id);
            if (profil == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(profil);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"profil sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja profila {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}