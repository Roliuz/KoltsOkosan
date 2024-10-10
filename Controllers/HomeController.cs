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

            // Számítsd ki az összes költség összegét
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // Tárold az adatokat egy ViewModel-ben vagy ViewBag-ben
            ViewBag.Osszeg = osszeg;

            return View(kiadasok);
        }

        public IActionResult KiadasTorles(int id)
        {
            var kiadasokIdKereses = _tartalom.Kiadasok.Find(id); //megkeresem a kért id-t
            if (kiadasokIdKereses != null) //ha van valamilyen értéke
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
            return View();  // Ez megjeleníti az ûrlap nézetét
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UjKiadas(Kiadas ujKiadas)
        {
            if (ModelState.IsValid)
            {
                _tartalom.Kiadasok.Add(ujKiadas); // Adatok hozzáadása
                _tartalom.SaveChanges(); // Mentés az adatbázisba
                return RedirectToAction(nameof(Index)); // Visszairányítás az index nézetre
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

            // Számítsd ki a szûrt eredmények összegét
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // Tárold az összeget ViewBag-ben
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
            // Számítsd ki a szûrt eredmények összegét
            var osszeg = kiadasok.Sum(k => k.Osszeg);

            // Tárold az összeget ViewBag-ben
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
