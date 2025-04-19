using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Models;

namespace Minimarket.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoIntegradorContext _context;

        public static List<Detalle> detalles = new List<Detalle>();
        static double sumaTotal = 0.0;

        public HomeController(ILogger<HomeController> logger, ProyectoIntegradorContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Carrito()
        {

            ViewBag.CarritoCount = detalles.Count;
            ViewBag.Total = sumaTotal;
            return View(detalles);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
