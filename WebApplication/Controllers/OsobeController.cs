using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class OsobeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public OsobeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Osobe.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Osobe, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdOsobe;
                    break;
                case 2:
                    orderSelector = b => b.Ime;
                    break;
                case 3:
                    orderSelector = b => b.Prezime;
                    break;   
                case 4:
                    orderSelector = b => b.DatumRodjenja;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var osobe =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new OsobeViewModel()
            {
                Osobe = osobe,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Osobe osoba)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(osoba);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"osoba {osoba.Ime} {osoba.Prezime} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(osoba);
                }
            }
            else
            {
                return View(osoba);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var osoba = _ctx.Osobe
                            .AsNoTracking()
                            .Where(b => b.IdOsobe == id)
                            .FirstOrDefault();
            if (osoba == null)
            {
                return NotFound($"Ne postoji osoba s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(osoba);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Osobe osoba = await _ctx.Osobe.FindAsync(id);
                if (osoba == null)
                {
                    return NotFound($"Ne postoji osoba s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Osobe>(osoba,
                    "", b=>b.IdOsobe, b=>b.Ime, b=> b.Prezime, b=>b.DatumRodjenja);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"osoba {osoba.Ime} {osoba.Prezime} uspjesno azuriran.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(osoba);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o osobi nije moguce povezati.");
                    return View(osoba);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja osobe." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var osoba = await _ctx.Osobe.FindAsync(id);
            if (osoba == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    _ctx.Remove(osoba);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Osoba uspjesno obrisana.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja osobe." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}