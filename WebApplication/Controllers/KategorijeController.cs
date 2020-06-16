using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize]
    public class KategorijeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public KategorijeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Kategorije.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Kategorije, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = b => b.IdKategorije;
                    break;
                case 2:
                    orderSelector = b => b.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var kategorije =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new KategorijeViewModel()
            {
                Kategorije = kategorije,
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
        public async Task<IActionResult> Create(Kategorije kategorija)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ctx.AddAsync(kategorija);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Kategorija {kategorija.Naziv} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(kategorija);
                }
            }
            else
            {
                return View(kategorija);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var kategorija = _ctx.Kategorije
                            .AsNoTracking()
                            .Where(b => b.IdKategorije == id)
                            .FirstOrDefault();
            if (kategorija == null)
            {
                return NotFound($"Ne postoji kategorija s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(kategorija);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Kategorije kategorija = await _ctx.Kategorije.FindAsync(id);
                if (kategorija == null)
                {
                    return NotFound($"Ne postoji kategorija s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Kategorije>(kategorija,
                    "", b=>b.IdKategorije, b=>b.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Kategorija {kategorija.Naziv} uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(kategorija);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o kategoriji nije moguce povezati.");
                    return View(kategorija);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja kategorije." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var kategorija = await _ctx.Kategorije.FindAsync(id);
            if (kategorija == null)
            {
                return NotFound(id);
            }
            else
            {
                try
                {
                    string naziv = kategorija.Naziv;
                    _ctx.Remove(kategorija);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Kategorija {naziv} uspjesno obrisana.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja kategorije." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var kategorija = await _ctx.Kategorije.FindAsync(id);
            var usluge = await _ctx.Usluge.Where(d=>d.IdKategorije == kategorija.IdKategorije).ToListAsync();
            ViewBag.Usluge = usluge;
            return View(kategorija);
        }
    }
}