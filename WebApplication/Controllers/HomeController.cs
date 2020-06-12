using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PI10Context _ctx;
        private readonly AppSettings _appSettings;


        public HomeController(ILogger<HomeController> logger,PI10Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _logger = logger;
            _ctx = ctx;
            _appSettings = optionsSnapshot.Value;

        }

        public IActionResult Index()
        {
            var kategorije = _ctx.Kategorije.AsNoTracking().ToList();
            if (kategorije.Any())
            {
                return View(kategorije);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}