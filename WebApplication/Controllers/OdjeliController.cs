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
    [Authorize]
    public class OdjeliController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public OdjeliController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Odjeli.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Odjeli, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdOdjela;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var odjeli =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new OdjeliViewModel()
            {
                Odjeli = odjeli,
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
        public IActionResult Create(Odjeli odjel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(odjel);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Odjel {odjel.Naziv} uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(odjel);
                }
            }
            else
            {
                return View(odjel);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var odjel = _ctx.Odjeli
                            .AsNoTracking()
                            .Where(b => b.IdOdjela == id)
                            .FirstOrDefault();
            if (odjel == null)
            {
                return NotFound($"Ne postoji odjel s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(odjel);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Odjeli odjel = await _ctx.Odjeli.FindAsync(id);
                if (odjel == null)
                {
                    return NotFound($"Ne postoji odjel s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Odjeli>(odjel,
                    "", b=>b.IdOdjela, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"odjel {odjel.Naziv} uspjesno azuriran.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(odjel);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o odjelu nije moguce povezati.");
                    return View(odjel);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja odjela." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var odjel = await _ctx.Odjeli.FindAsync(id);
            if (odjel == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    string naziv = odjel.Naziv;
                    _ctx.Remove(odjel);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"odjel {naziv} uspjesno obrisan.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja odjela." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}