﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class TvrtkeController : Controller
    {
        private readonly PI10Context _ctx;
        private readonly ILogger<TvrtkeController> _logger;
        private readonly AppSettings _appSettings;

        public TvrtkeController(PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot,
            ILogger<TvrtkeController> logger)
        {
            this._ctx = ctx;
            _appSettings = optionsSnapshot.Value;
            _logger = logger;
        }


        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = _appSettings.PageSize;
            var query = _ctx.Tvrtke.AsNoTracking();
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

            System.Linq.Expressions.Expression<Func<Tvrtke, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.IdTvrtke;
                    break;
                case 2:
                    orderSelector = d => d.Naziv;
                    break;
                case 3:
                    orderSelector = d => d.DogovoreniPopustPostotak;
                    break;
                case 4:
                    orderSelector = d => d.VrijemeSuradnjeGodine;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }

            var tvrtke =query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var modelD = new TvrtkeViewModel
            {
                Tvrtke = tvrtke,
                PagingInfo = pagingInfo
            };
            return View(modelD);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<bool> ProvjeriSifruArtikla(int idTvrtke)
        {
            bool exists = await _ctx.Tvrtke.AnyAsync(a => a.IdTvrtke == idTvrtke);
            return !exists;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tvrtke tvrtka)
        {
            bool exists = await _ctx.Tvrtke.AnyAsync(d => d.IdTvrtke == tvrtka.IdTvrtke);
            if (exists)
            {
                ModelState.AddModelError(nameof(tvrtka.IdTvrtke), "tvrtka s navedenom sifrom vec postoji!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Add(tvrtka);
                    await _ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"tvrtka {tvrtka.IdTvrtke} uspjesno dodana.*";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(tvrtka);
                }
            }
            else
            {
                return View(tvrtka);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var tvrtka = _ctx.Tvrtke.Find(id);
            if (tvrtka == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _ctx.Remove(tvrtka);
                    _ctx.SaveChanges();
                    var result = new
                    {
                        message = $"tvrtka sa sifrom {id} obrisana.",
                        successful = true
                    };
                    return Json(result);
                }
                catch (Exception e)
                {
                    var result = new
                    {
                        message = $"Pogreska prilikom bisanja tvrtke {e.Message}",
                        successful = false
                    };
                    return Json(result);
                }
            }
        }

        public PartialViewResult Row(int id)
        {
            var tvrtka = _ctx.Tvrtke.Find(id);
            if (tvrtka != null)
            {
                return PartialView(tvrtka);
            }
            else
            {
                return PartialView("ErrorMessageRow", $"Neispravna sifra tvrtke : {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tvrtka = await _ctx.Tvrtke.FindAsync(id);
            if (tvrtka != null)
            {
                return PartialView(tvrtka);
            }
            else
            {
                return NotFound($"Neispravna sifra tvrtka: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tvrtke tvrtka)
        {
            if (tvrtka == null)
            {
                return NotFound("Nema poslanih podataka");
            } 
            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(tvrtka);
                    _ctx.SaveChanges();
                    return StatusCode(302, Url.Action(nameof(Row), new {id = tvrtka.IdTvrtke}));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                    return PartialView(tvrtka);
                }
            }
            else
            {
                return PartialView(tvrtka);
            }
        }
    }
}