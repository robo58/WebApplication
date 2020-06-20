using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Areas.AutoComplete.Models;
using WebApplication.Models;

namespace WebApplication.Areas.AutoComplete.Controllers
{
    
    [Area("AutoComplete")]
    public class VrsteGorivaController : Controller
    {
        private readonly PI10Context _context;
        private readonly AppSettings appData;
        
        public VrsteGorivaController(PI10Context context, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _context = context;
            appData = optionsSnapshot.Value;
        }

        public IEnumerable<IdLabel> Get(string term)
        {
            var query = _context.VrsteGoriva.Select(d => new IdLabel
            {
                Id = d.IdVrsteGoriva,
                Label = d.Naziv
            }).Where(l=>l.Label.Contains(term));

            var list = query.OrderBy(l => l.Label).Take(appData.AutoCompleteCount)
                .ToList();

            return list;
        }
    }
}