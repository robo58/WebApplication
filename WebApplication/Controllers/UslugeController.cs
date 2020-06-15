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
    public class UslugeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public UslugeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Usluge.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Usluge, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdUsluge;
                    break;
                case 2:
                    orderSelector = b => b.NazivUsluge;
                    break;
                case 3:
                    orderSelector = b => b.IdKategorijeNavigation.Naziv;
                    break; ;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var usluge =query
                .Select(s=>new UslugaViewModel
                {
                    IdUsluge = s.IdUsluge,
                    NazivKategorije = s.IdKategorijeNavigation.Naziv,
                    NazivUsluge = s.NazivUsluge
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new UslugeViewModel()
            {
                Usluge = usluge,
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
            var kat = _ctx.Kategorije.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdKategorije}).ToList();
            
            ViewBag.Kategorije = new SelectList(kat, nameof(Kategorije.IdKategorije), nameof(Kategorije.Naziv));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usluge usluga)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(usluga);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"usluga uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(usluga);
                }
            }
            else
            {
                return View(usluga);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var usluga = _ctx.Usluge
                            .AsNoTracking()
                            .Where(b => b.IdUsluge == id)
                            .FirstOrDefault();
            if (usluga == null)
            {
                return NotFound($"Ne postoji usluga s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(usluga);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usluge usluga, int page = 1, int sort = 1, bool ascending = true)
        {
            if (usluga == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Usluge.Any(m => m.IdUsluge == usluga.IdUsluge);
            if (!checkId)
            {
                return NotFound($"Neispravan id usluga: {usluga?.IdUsluge}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(usluga);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "usluga ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(usluga);
                }
            }
            else
            {
                return View(usluga);
            }
        }

        public IActionResult Row(int id)
        {
            var usluga = _ctx.Usluge                       
                .Where(s => s.IdUsluge == id)
                .Select(s => new UslugaViewModel()
                {
                    IdUsluge = s.IdUsluge,
                    NazivKategorije = s.IdKategorijeNavigation.Naziv,
                    NazivUsluge = s.NazivUsluge
                })
                .SingleOrDefault();
            if (usluga != null)
            {
                return PartialView(usluga);
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
            var usluga = await _ctx.Usluge.FindAsync(id);
            if (usluga == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(usluga);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"usluga sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja usluga {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var usluga = await _ctx.Usluge.FindAsync(id);
            var vozila = await _ctx.Vozila.Where(d => d.IdModelaNavigation.Tip == usluga.NazivUsluge)
                .Select(v=> new VozilosViewModel
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
                            NazivDodatneOpreme = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdDodatneOpremeNavigation.ToString(),
                            NazivMjenjaca = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdMjenjacaNavigation.Naziv,
                            NazivVrsteGoriva = v.IdModelaNavigation.IdSpecifikacijaNavigation.IdVrsteGorivaNavigation.Naziv,
                            Potrosnja = v.IdModelaNavigation.IdSpecifikacijaNavigation.Potrosnja,
                            VelicinaTankaULitrima = v.IdModelaNavigation.IdSpecifikacijaNavigation.VelicinaTankaULitrima
                        }
                    },
                    NazivProizvodjaca = v.IdProizvodjacaNavigation.Naziv,
                    IdSlike = v.IdSlike
                })
                .ToListAsync();
            ViewBag.Vozila = vozila;
            return View(usluga);
        }
        
    }
}