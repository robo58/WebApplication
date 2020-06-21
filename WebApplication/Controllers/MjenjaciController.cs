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
    public class MjenjaciController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public MjenjaciController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Mjenjaci.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Mjenjaci, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdMjenjaca;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var mjenjaci =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new MjenjaciViewModel()
            {
                Mjenjaci = mjenjaci,
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
        public IActionResult Create(Mjenjaci mjenjac)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(mjenjac);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Mjenjac {mjenjac.Naziv} uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    TempData[Constants.ErrorOccurred] = $"Greska: {e.Message + e.StackTrace}";
                    return View(mjenjac);
                }
            }
            else
            {
                return View(mjenjac);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var mjenjac = _ctx.Mjenjaci
                            .AsNoTracking()
                            .Where(b => b.IdMjenjaca == id)
                            .FirstOrDefault();
            if (mjenjac == null)
            {
                return NotFound($"Ne postoji mjenjac s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(mjenjac);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Mjenjaci mjenjac = await _ctx.Mjenjaci.FindAsync(id);
                if (mjenjac == null)
                {
                    return NotFound($"Ne postoji mjenjac s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Mjenjaci>(mjenjac,
                    "", b=>b.IdMjenjaca, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Mjenjac {mjenjac.Naziv} uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(mjenjac);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o mjenjacu nije moguce povezati.");
                    return View(mjenjac);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja Mjenjaca." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var mjenjac = await _ctx.Mjenjaci.FindAsync(id);
            if (mjenjac == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    string naziv = mjenjac.Naziv;
                    _ctx.Remove(mjenjac);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Mjenjac {naziv} uspjesno obrisan.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja mjenjaca." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
    }
}