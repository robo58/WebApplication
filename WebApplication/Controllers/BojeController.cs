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
    public class BojeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public BojeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Boje.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Boje, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdBoje;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var boje =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new BojeViewModel()
            {
                Boje = boje,
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
        public IActionResult Create(Boje boja)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(boja);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Drzava {boja.Naziv} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(boja);
                }
            }
            else
            {
                return View(boja);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var boja = _ctx.Boje
                            .AsNoTracking()
                            .Where(b => b.IdBoje == id)
                            .FirstOrDefault();
            if (boja == null)
            {
                return NotFound($"Ne postoji drzava s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(boja);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Boje boja = await _ctx.Boje.FindAsync(id);
                if (boja == null)
                {
                    return NotFound($"Ne postoji drzava s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Boje>(boja,
                    "", b=>b.IdBoje, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Boja {boja.Naziv} uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(boja);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o drzavi nije moguce povezati.");
                    return View(boja);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja Boje." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int IdBoje, int page = 1, int sort = 1, bool ascending = true)
        {
            var boja = await _ctx.Boje.FindAsync(IdBoje);
            if (boja == null)
            {
                return NotFound(IdBoje);
            }
            else
            {
                try
                {
                    string naziv = boja.Naziv;
                    _ctx.Remove(boja);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Boja {naziv} uspjesno obrisana.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja boje." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}