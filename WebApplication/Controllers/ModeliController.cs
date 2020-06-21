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
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "admin,zaposlenik")]
    public class ModeliController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public ModeliController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Modeli.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Modeli, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdModela;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
                case 3:
                    orderSelector = b => b.IdSpecifikacijaNavigation.KonjskeSnage;
                    break;
                case 4:
                    orderSelector = b => b.IdSpecifikacijaNavigation.IdMjenjacaNavigation.Naziv;
                    break;
                case 5:
                    orderSelector = b => b.IdSpecifikacijaNavigation.IdBojeNavigation.Naziv;
                    break;
                case 6:
                    orderSelector = b => b.IdSpecifikacijaNavigation.IdDodatneOpreme;
                    break;
                case 7:
                    orderSelector = b => b.IdSpecifikacijaNavigation.IdVrsteGorivaNavigation.Naziv;
                    break;
                case 8:
                    orderSelector = b => b.IdSpecifikacijaNavigation.VelicinaTankaULitrima;
                    break;
                case 9:
                    orderSelector = b => b.IdSpecifikacijaNavigation.Potrosnja;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var modeli =query
                .Select(s=>new ModelViewModel
                {
                    IdModela = s.IdModela,
                    Naziv = s.Naziv,
                    Specifikacija =  new SpecifikacijaViewModel
                    {
                        IdSpecifikacija = s.IdSpecifikacijaNavigation.IdSpecifikacija,
                        KonjskeSnage = s.IdSpecifikacijaNavigation.KonjskeSnage,
                        NazivMjenjaca = s.IdSpecifikacijaNavigation.IdMjenjacaNavigation.Naziv,
                        NazivBoje = s.IdSpecifikacijaNavigation.IdBojeNavigation.Naziv,
                        NazivDodatneOpreme = s.IdSpecifikacijaNavigation.IdDodatneOpremeNavigation.ToString(),
                        NazivVrsteGoriva = s.IdSpecifikacijaNavigation.IdVrsteGorivaNavigation.Naziv,
                        VelicinaTankaULitrima = s.IdSpecifikacijaNavigation.VelicinaTankaULitrima,
                        Potrosnja = s.IdSpecifikacijaNavigation.Potrosnja
                    }
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new ModeliViewModel()
            {
                Modeli = modeli,
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
            var specs = _ctx.Specifikacije.AsNoTracking().OrderBy(d => d.IdSpecifikacija)
                .Select(d => new {info = $"(KS={d.KonjskeSnage}, Mjenjac={d.IdMjenjacaNavigation.Naziv}, Boja={d.IdBojeNavigation.Naziv}, " +
                                         $"DodatnaOprema={d.IdDodatneOpremeNavigation.ToString()}, Gorivo={d.IdVrsteGorivaNavigation.Naziv}, " +
                                         $"Velicina tanka={d.VelicinaTankaULitrima}, Potrosnja={d.Potrosnja})", d.IdSpecifikacija}).ToList();
            
            ViewBag.Specifikacije = new SelectList(specs, nameof(Specifikacije.IdSpecifikacija), "info");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Modeli model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(model);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"model uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var model = _ctx.Modeli
                            .AsNoTracking()
                            .Where(b => b.IdModela == id)
                            .FirstOrDefault();
            if (model == null)
            {
                return NotFound($"Ne postoji model s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(model);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Modeli model, int page = 1, int sort = 1, bool ascending = true)
        {
            if (model == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Modeli.Any(m => m.IdModela == model.IdModela);
            if (!checkId)
            {
                return NotFound($"Neispravan id modela: {model?.IdModela}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(model);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "model ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Row(int id)
        {
            var model = _ctx.Modeli                       
                .Where(s => s.IdModela == id)
                .Select(s=>new ModelViewModel
                {
                    IdModela = s.IdModela,
                    Naziv = s.Naziv,
                    Specifikacija =  new SpecifikacijaViewModel
                    {
                        IdSpecifikacija = s.IdSpecifikacijaNavigation.IdSpecifikacija,
                        KonjskeSnage = s.IdSpecifikacijaNavigation.KonjskeSnage,
                        NazivMjenjaca = s.IdSpecifikacijaNavigation.IdMjenjacaNavigation.Naziv,
                        NazivBoje = s.IdSpecifikacijaNavigation.IdBojeNavigation.Naziv,
                        NazivDodatneOpreme = s.IdSpecifikacijaNavigation.IdDodatneOpremeNavigation.ToString(),
                        NazivVrsteGoriva = s.IdSpecifikacijaNavigation.IdVrsteGorivaNavigation.Naziv,
                        VelicinaTankaULitrima = s.IdSpecifikacijaNavigation.VelicinaTankaULitrima,
                        Potrosnja = s.IdSpecifikacijaNavigation.Potrosnja
                    }
                })
                .SingleOrDefault();
            if (model != null)
            {
                return PartialView(model);
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
            var model = await _ctx.Modeli.FindAsync(id);
            if (model == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(model);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"usluga sa model {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja model {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}