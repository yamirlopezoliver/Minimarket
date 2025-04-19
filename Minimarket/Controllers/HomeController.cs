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
        static double? sumaTotal = 0.0;

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

        [HttpPost]
        public async Task<IActionResult> Carrito(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            Detalle detalle = new Detalle
            {
                Cantidad = cantidad,
                Precio = producto.Precio,
                Total = cantidad * producto.Precio,
                Producto = producto
            };

            // Verifica si el producto ya está en la lista
            var detalleExistente = detalles.FirstOrDefault(d => d.Producto?.Id == producto.Id);
            if (detalleExistente != null)
            {   // Si ya existe, actualiza la cantidad y el total
                detalleExistente.Cantidad += detalle.Cantidad;
                detalleExistente.Total = detalleExistente.Cantidad * detalleExistente.Precio;
            }
            else
            detalles.Add(detalle);

            sumaTotal = detalles.Sum(s => s.Total);
            ViewBag.Total = sumaTotal;

            return RedirectToAction("Carrito", "Home");
        }

        public IActionResult DeleteCart(int id)
        {
            List<Detalle> detallesNueva = new List<Detalle>();
            foreach (var item in detalles)
            {
                if (item.Producto.Id != id)
                {
                    detallesNueva.Add(item);
                }
                detalles = detallesNueva;
                sumaTotal = detalles.Sum(s => s.Total);

            }
            TempData["Message"] = "Producto eliminado del carrito.";
            TempData["MessageType"] = "warning"; // success, error, info, warning

            return RedirectToAction("Carrito", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
