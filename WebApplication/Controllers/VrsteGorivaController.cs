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
    public class VrsteGorivaController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public VrsteGorivaController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.VrsteGoriva.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<VrsteGoriva, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdVrsteGoriva;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var vrsteGoriva =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new VrsteGorivaViewModel()
            {
                VrsteGoriva = vrsteGoriva,
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
        public IActionResult Create(VrsteGoriva vrsteGoriva)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(vrsteGoriva);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Drzava {vrsteGoriva.Naziv} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(vrsteGoriva);
                }
            }
            else
            {
                return View(vrsteGoriva);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vrsteGoriva = _ctx.VrsteGoriva
                            .AsNoTracking()
                            .Where(b => b.IdVrsteGoriva == id)
                            .FirstOrDefault();
            if (vrsteGoriva == null)
            {
                return NotFound($"Ne postoji vrsteGoriva s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vrsteGoriva);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                VrsteGoriva vrsteGoriva = await _ctx.VrsteGoriva.FindAsync(id);
                if (vrsteGoriva == null)
                {
                    return NotFound($"Ne postoji vrsteGoriva s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<VrsteGoriva>(vrsteGoriva,
                    "", b=>b.IdVrsteGoriva, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"vrsteGoriva {vrsteGoriva.Naziv} uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(vrsteGoriva);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o vrsteGoriva nije moguce povezati.");
                    return View(vrsteGoriva);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja vrsteGoriva." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vrsteGoriva = await _ctx.Boje.FindAsync(id);
            if (vrsteGoriva == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    string naziv = vrsteGoriva.Naziv;
                    _ctx.Remove(vrsteGoriva);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"vrsteGoriva {naziv} uspjesno obrisana.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja vrsteGoriva." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}