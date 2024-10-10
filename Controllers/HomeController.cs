using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using UjGyakorlas.Models;

namespace UjGyakorlas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KiadasokDbContext _tartalom;

        public HomeController(ILogger<HomeController> logger, KiadasokDbContext tartalom)
        {
            _logger = logger;
            _tartalom = tartalom;
        }



        public IActionResult Index()
        {
            var kiadasok = _tartalom.Kiadasok.ToList();

            // Sz�m�tsd ki az �sszes k�lts�g �sszeg�t
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // T�rold az adatokat egy ViewModel-ben vagy ViewBag-ben
            ViewBag.Osszeg = osszeg;

            return View(kiadasok);
        }

        public IActionResult KiadasTorles(int id)
        {
            var kiadasokIdKereses = _tartalom.Kiadasok.Find(id); //megkeresem a k�rt id-t
            if (kiadasokIdKereses != null) //ha van valamilyen �rt�ke
            {
                _tartalom.Kiadasok.Remove(kiadasokIdKereses);
                _tartalom.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult KiadasModosit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kiadas = _tartalom.Kiadasok.Find(id);
            if (kiadas == null)
            {
                return NotFound();
            }

            return View(kiadas);
        }
        [HttpPost]
        public IActionResult KiadasModosit(Kiadas kiadas)
        {
            if (ModelState.IsValid)
            {
                _tartalom.Update(kiadas);
                _tartalom.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(kiadas);
        }

      

        public IActionResult UjKiadas()
        {
            return View();  // Ez megjelen�ti az �rlap n�zet�t
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UjKiadas(Kiadas ujKiadas)
        {
            if (ModelState.IsValid)
            {
                _tartalom.Kiadasok.Add(ujKiadas); // Adatok hozz�ad�sa
                _tartalom.SaveChanges(); // Ment�s az adatb�zisba
                return RedirectToAction(nameof(Index)); // Visszair�ny�t�s az index n�zetre
            }
            return View(ujKiadas);
        }

        [HttpGet]
        public IActionResult Szures(DateTime? fromDate, DateTime? toDate)
        {
            var kiadasok = _tartalom.Kiadasok.AsQueryable();

            if (fromDate.HasValue)
            {
                kiadasok = kiadasok.Where(k => k.Datum >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                kiadasok = kiadasok.Where(k => k.Datum <= toDate.Value);
            }

            // Sz�m�tsd ki a sz�rt eredm�nyek �sszeg�t
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // T�rold az �sszeget ViewBag-ben
            ViewBag.Osszeg = osszeg;


            return View("Index", kiadasok.ToList());
        }

        [HttpGet]
        public IActionResult KatSzures(string Kategoria)
        {
            var kiadasok = _tartalom.Kiadasok.AsQueryable();

            if(!string.IsNullOrEmpty(Kategoria))
            {
                kiadasok = kiadasok.Where(v => v.Tipus == Kategoria);
            }
            // Sz�m�tsd ki a sz�rt eredm�nyek �sszeg�t
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // T�rold az �sszeget ViewBag-ben
            ViewBag.Osszeg = osszeg;

            return View("Index",kiadasok.ToList());
        }

        [HttpGet]
        public JsonResult Kereses(string query)
        {
            var eredmenyek = _tartalom.Kiadasok
                .Where(k => string.IsNullOrEmpty(query) ||
                            k.Cim.ToLower().Contains(query.ToLower()) ||
                            k.Tipus.ToLower().Contains(query.ToLower()))
                .Select(k => new
                {
                    cim = k.Cim,
                    tipus = k.Tipus,
                    osszeg = k.Osszeg,
                    datum = k.Datum.ToShortDateString(),
                    id = k.Id
                })
                .ToList();

            return Json(eredmenyek);
        }


        public IActionResult SzuresVisszaAllitas()
        {
            var kiadasok = _tartalom.Kiadasok.ToList();

            return View("Index", kiadasok);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
