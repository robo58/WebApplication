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
    public class VozilaController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public VozilaController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Vozila.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Vozila, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdVozila;
                    break;
                case 2:
                    orderSelector = b => b.IdProizvodjacaNavigation.Naziv;
                    break;
                case 3:
                    orderSelector = b => b.IdModelaNavigation.Naziv;
                    break;
                case 4:
                    orderSelector = b => b.Cijena;
                    break;
                case 5:
                    orderSelector = b => b.Dostupno;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var vozila =query
                .Select(s=>new VoziloViewModel
                {
                    IdVozila = s.IdVozila,
                    NazivProizvodjaca = s.IdProizvodjacaNavigation.Naziv,
                    NazivModela = s.IdModelaNavigation.Naziv,
                    Cijena = s.Cijena,
                    Dostupno = s.Dostupno
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new VozilaViewModel
            {
                Vozila = vozila,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }
        
        [HttpGet]
        public IActionResult Show(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vozilo = _ctx.Vozila
                .AsNoTracking()
                .Where(b => b.IdVozila == id)
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
                    NazivProizvodjaca = v.IdProizvodjacaNavigation.Naziv
                })
                .FirstOrDefault();

            if (vozilo == null)
            {
                return NotFound($"Ne postoji vozilo s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vozilo);
            }
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            PrepareDropDownLists();
            return View();
        }
        
        private void PrepareDropDownLists()
        {
            var proizvodjaci = _ctx.Proizvodjaci.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdProizvodjaca}).ToList();
            var modeli = _ctx.Modeli.AsNoTracking().OrderBy(d => d.Naziv)
                .Select(d => new {d.Naziv, d.IdModela}).ToList();

            ViewBag.Proizvodjaci = new SelectList(proizvodjaci, nameof(Proizvodjaci.IdProizvodjaca), nameof(Proizvodjaci.Naziv));
            ViewBag.Modeli = new SelectList(modeli, nameof(Modeli.IdModela), nameof(Modeli.Naziv));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vozila vozilo, IFormFile slika)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (slika != null && slika.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            await slika.CopyToAsync(stream);
                            byte[] image = stream.ToArray();
                            int GetId()
                            {
                                int i = 0;
                                if (_ctx.Slike.Any())
                                {
                                    i = _ctx.Slike.OrderBy(d=>d.IdSlike).Last().IdSlike + 1;
                                }
                                return i;
                            }
                            var slike = new Slike
                            {
                                IdSlike = GetId(),
                                SlikaBinary = image
                            };
                            _ctx.Add(slike);
                            vozilo.IdSlike = slike.IdSlike;
                        }
                    }
                    _ctx.Add(vozilo);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"vozilo uspjesno dodano.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    PrepareDropDownLists();
                    return View(vozilo);
                }
            }
            else
            {
                return View(vozilo);
            }
        }
        
        public async Task<FileContentResult> GetImage(int id)
        {
            byte[] image = await _ctx.Slike.Where(a => a.IdSlike == id)
                .Select(a => a.SlikaBinary)
                .SingleOrDefaultAsync();

            if (image != null)
            {
                return File(image, "image/jpeg");
            }
            else
            {
                return null;
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vozilo = _ctx.Vozila
                            .AsNoTracking()
                            .Where(b => b.IdVozila == id)
                            .FirstOrDefault();
            if (vozilo == null)
            {
                return NotFound($"Ne postoji vozilo s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View(vozilo);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vozila vozilo, int page = 1, int sort = 1, bool ascending = true)
        {
            if (vozilo == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = _ctx.Vozila.Any(m => m.IdVozila == vozilo.IdVozila);
            if (!checkId)
            {
                return NotFound($"Neispravan id vozila: {vozilo?.IdVozila}");
            }

            PrepareDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(vozilo);
                    _ctx.SaveChanges();

                    TempData[Constants.Message] = "vozilo ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });          
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(vozilo);
                }
            }
            else
            {
                return View(vozilo);
            }
        }

        public IActionResult Row(int id)
        {
            var vozilo = _ctx.Vozila                       
                .Where(s => s.IdVozila == id)
                .Select(s=>new VoziloViewModel
                {
                    IdVozila = s.IdVozila,
                    NazivProizvodjaca = s.IdProizvodjacaNavigation.Naziv,
                    NazivModela = s.IdModelaNavigation.Naziv,
                    Cijena = s.Cijena,
                    Dostupno = s.Dostupno
                })
                .SingleOrDefault();
            if (vozilo != null)
            {
                return PartialView(vozilo);
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
            var vozilo = await _ctx.Vozila.FindAsync(id);
            if (vozilo == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(vozilo);
                    await _ctx.SaveChangesAsync();
                    var result = new
                    {
                        message = $"vozilo sa sifrom {id} obrisan.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja voziloa {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }
        
    }
}