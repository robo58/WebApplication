using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "admin,zaposlenik")]
    public class ProizvodjaciController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public ProizvodjaciController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Proizvodjaci.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Proizvodjaci, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdProizvodjaca;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var proizvodjaci =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new ProizvodjaciViewModel()
            {
                Proizvodjaci = proizvodjaci,
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
        public IActionResult Create(Proizvodjaci proizvodjac)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(proizvodjac);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"proizvodjac {proizvodjac.Naziv} uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(proizvodjac);
                }
            }
            else
            {
                return View(proizvodjac);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var proizvodjac = _ctx.Proizvodjaci
                            .AsNoTracking()
                            .Where(b => b.IdProizvodjaca == id)
                            .FirstOrDefault();
            if (proizvodjac == null)
            {
                return NotFound($"Ne postoji proizvodjac s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(proizvodjac);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Proizvodjaci proizvodjac = await _ctx.Proizvodjaci.FindAsync(id);
                if (proizvodjac == null)
                {
                    return NotFound($"Ne postoji proizvodjac s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Proizvodjaci>(proizvodjac,
                    "", b=>b.IdProizvodjaca, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"proizvodjac {proizvodjac.Naziv} uspjesno azuriran.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(proizvodjac);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o proizvodjacu nije moguce povezati.");
                    return View(proizvodjac);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja proizvodjaca." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var proizvodjac = await _ctx.Proizvodjaci.FindAsync(id);
            if (proizvodjac == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    string naziv = proizvodjac.Naziv;
                    _ctx.Remove(proizvodjac);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"proizvodjac {naziv} uspjesno obrisan.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja proizvodjaca." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}