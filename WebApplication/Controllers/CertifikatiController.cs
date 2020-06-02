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
    public class CertifikatiController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;

        public CertifikatiController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }
        
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Certifikati.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Certifikati, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = c => c.IdCertifikata;
                    break;
                case 2:
                    orderSelector = c => c.Naziv;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            
            var certifikati =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new CertifikatiViewModel()
            {
                Certifikati = certifikati,
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
        public IActionResult Create(Certifikati certifikati)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(certifikati);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Certifikat {certifikati.Naziv} uspjesno dodan.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(certifikati);
                }
            }
            else
            {
                return View(certifikati);
            }
        }
        
        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var certifikati = _ctx.Certifikati
                            .AsNoTracking()
                            .Where(c => c.IdCertifikata == id)
                            .FirstOrDefault();
            if (certifikati == null)
            {
                return NotFound($"Ne postoji certifikat s oznakom {id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(certifikati);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Certifikati certifikati = await _ctx.Certifikati.FindAsync(id);
                if (certifikati == null)
                {
                    return NotFound($"Ne postoji certifikat s oznakom {id}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Certifikati>(certifikati,
                    "", c=>c.IdCertifikata, c=>c.Naziv);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Certifikat {certifikati.Naziv} uspjesno azurirana.*";
                        TempData[Constants.ErrorOccurred] = false;
                        await _ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new {page, sort, ascending});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                        return View(certifikati);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o certifikatu nije moguce povezati.");
                    return View(certifikati);
                }
            }
            catch (Exception e)
            {
                TempData[Constants.Message] = "Pogreska prilikom azuriranja certifikata." + e.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new {id, page, sort, ascending});
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int IdCertifikata, int page = 1, int sort = 1, bool ascending = true)
        {
            var certifikat = _ctx.Certifikati.Find(IdCertifikata);
            if (certifikat == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    string naziv = certifikat.Naziv;
                    _ctx.Remove(certifikat);
                    _ctx.SaveChanges();
                    TempData[Constants.Message] = $"Certifikat {naziv} uspjesno obrisana.*";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception e)
                {
                    TempData[Constants.Message] = "Pogreska prilikom brisanja certifikata." + e.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index), new {page, sort, ascending});
            }
        }
    }
}