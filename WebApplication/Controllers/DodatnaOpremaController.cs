﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
 using WebApplication.Models;
 using WebApplication.ViewModels;

 namespace WebApplication.Controllers
{
    [Authorize]
    public class DodatnaOpremaController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly ILogger<DodatnaOpremaController> _logger;
        private readonly AppSettings _appSettings;

        public DodatnaOpremaController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot,
            ILogger<DodatnaOpremaController> logger)
        {
            this._ctx = ctx;
            _appSettings = optionsSnapshot.Value;
            _logger = logger;
        }


        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.DodatnaOprema.AsNoTracking();
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
                return RedirectToAction(nameof(Index), new {page = pagingInfo.TotalPages, sort, ascending});
            }

            System.Linq.Expressions.Expression<Func<DodatnaOprema, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.IdDodatneOpreme;
                    break;
                case 2:
                    orderSelector = d => d.Siber;
                    break;
                case 3:
                    orderSelector = d => d.Klima;
                    break;
                case 4:
                    orderSelector = d => d.KozniSicevi;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }

            var oprema =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new DodatneOpremeViewModel
            {
                Oprema = oprema,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<bool> ProvjeriSifruArtikla(int IdDodatneOpreme)
        {
            bool exists = await _ctx.DodatnaOprema.AnyAsync(a => a.IdDodatneOpreme == IdDodatneOpreme);
            return !exists;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DodatnaOprema oprema)
        {
            bool exists = await _ctx.DodatnaOprema.AnyAsync(d => d.IdDodatneOpreme == oprema.IdDodatneOpreme);
            if (exists)
            {
                ModelState.AddModelError(nameof(oprema.IdDodatneOpreme), "Oprema s navedenom sifrom vec postoji!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(oprema);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Oprema {oprema.IdDodatneOpreme} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(oprema);
                }
            }
            else
            {
                return View(oprema);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var oprema = _ctx.DodatnaOprema.Find(id);
            if (oprema == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _ctx.Remove(oprema);
                    _ctx.SaveChanges();
                    var result = new
                    {
                        message = $"Oprema sa sifrom {id} obrisana.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja artikla {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }

        public PartialViewResult Row(int id)
        {
            var oprema = _ctx.DodatnaOprema.Find(id);
            if (oprema != null)
            {
                return PartialView(oprema);
            }
            else
            {
                return PartialView("ErrorMessageRow", $"Neispravna sifra artikla : {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var oprema = await _ctx.DodatnaOprema.FindAsync(id);
            if (oprema != null)
            {
                return PartialView(oprema);
            }
            else
            {
                return NotFound($"Neispravna sifra opreme: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DodatnaOprema oprema)
        {
            if (oprema == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            DodatnaOprema dbOprema = await _ctx.DodatnaOprema.FindAsync(oprema.IdDodatneOpreme);
            if (dbOprema == null)
            {
                return NotFound($"Neispravna sifra opreme: {oprema.IdDodatneOpreme}");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dbOprema.Siber = oprema.Siber;
                    dbOprema.Klima = oprema.Klima;
                    dbOprema.KozniSicevi = oprema.KozniSicevi;
                    await _ctx.SaveChangesAsync();
                    return StatusCode(302, Url.Action(nameof(Row), new {id = oprema.IdDodatneOpreme}));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                    return PartialView(oprema);
                }
            }
            else
            {
                return PartialView(oprema);
            }
        }
    }
}