using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CarritoCount = detalles.Count;
            ViewBag.Total = sumaTotal;
            return View(detalles);
        }

        [HttpPost]
        public async Task<IActionResult> Carrito(int id, int cantidad)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            Detalle detalle = new Detalle
            {
                Cantidad = cantidad,
                Precio = producto.Precio,
                Nombre = producto.Nombre,
                Total = TruncarADosDecimales(cantidad * producto.Precio),
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
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
            TempData["MessageType"] = "success"; // success, error, info, warning

            return RedirectToAction("Carrito", "Home");
        }

        public IActionResult Pago()
        {
            int? idUsuario = HttpContext.Session.GetInt32("UserId");
            if ( idUsuario == null)
            {
                TempData["Message"] = "Primero debe Loguearse para realizar el pedido.";
                TempData["MessageType"] = "warning";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public async Task<IActionResult> GenerarPago()
        {
            try
            {
                //obtener el numero correlativo
                List<Ordene> ordenes = await _context.Ordenes.ToListAsync();
                // Determinar el número mayor en las órdenes existentes
                int numero = ordenes.Any() ? ordenes.Max(o => int.Parse(o.Numero)) + 1 : 1;
                // Formatear el número con ceros a la izquierda
                string numeroCorrelativo = numero.ToString("D10"); // "D10" asegura que tendrá 10 dígitos

                int? idUsuario = HttpContext.Session.GetInt32("UserId");
                if (idUsuario == null)
                {
                    TempData["Message"] = "Primero debe Loguearse para realizar el pedido.";
                    TempData["MessageType"] = "warning";
                    return RedirectToAction("Login", "Account");
                }
                User? usuario = await _context.Users.FindAsync(idUsuario);
                if (usuario == null)
                {
                    TempData["Message"] = "El Usuario no existe.";
                    TempData["MessageType"] = "warning";
                    return RedirectToAction("Carrito", "Home");
                }

                Ordene orden = new Ordene
                {
                    Usuario = usuario,
                    FechaCreacion = DateTime.Now,
                    Total = TruncarADosDecimales(sumaTotal),
                    Numero = numeroCorrelativo
                };
                //guardamos orden
                _context.Add(orden);
                await _context.SaveChangesAsync();

                foreach (var item in detalles)
                {
                    var det = new Detalle
                    {
                        Cantidad = item.Cantidad,
                        Precio = item.Precio,
                        Total = item.Total,
                        Nombre = item.Nombre,
                        Orden = orden,
                        Producto = item.Producto
                    };
                    _context.Add(det);

                    // Actualizar el stock del producto
                    var producto = await _context.Productos.FindAsync(item.Producto.Id);
                    if (producto != null)
                    {
                        producto.Stock -= (int)item.Cantidad; // Reducir el stock
                        _context.Productos.Update(producto);
                    }
                }
                await _context.SaveChangesAsync();

                detalles = new List<Detalle>();

                TempData["Message"] = "El pago fue realizado con exito.";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Message"] = "No fue posible realizar el pago.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Carrito", "Home");
            }
        }

        public async Task<IActionResult> ListaOrden()
        {
            List<Ordene> listaOrdenes = new List<Ordene>();
            int? idUsuario = HttpContext.Session.GetInt32("UserId");
            if (idUsuario == null)
            {
                TempData["Message"] = "Debe iniciar sesión para ver sus órdenes.";
                TempData["MessageType"] = "warning";
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Users
                .Include(u => u.Ordenes) 
                .FirstOrDefaultAsync(u => u.Id == idUsuario);

            if (usuario == null)
            {
                TempData["Message"] = "Usuario no encontrado.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Index", "Home");
            }

            // Verificamos si el usuario es administrador
            if (HttpContext.Session.GetInt32("UserIdRol")==1)
            {
                listaOrdenes = await _context.Ordenes
                    .Include(o => o.Usuario)
                    .ToListAsync();
            }else{
                listaOrdenes = usuario.Ordenes.ToList();
            }

            return View(listaOrdenes);
        }

        public async Task<IActionResult> DetalleOrden(int id)
        {
            List<Detalle> lista = new List<Detalle>();
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                //obtiene detalles oasociados a Orden
                var orden = await _context.Ordenes.Include(u => u.Detalles).FirstOrDefaultAsync(d => d.Id == id);
                if (orden == null)
                {
                    return NotFound();
                }
                var usuario = await _context.Users.FindAsync(orden.UsuarioId);
                
                lista = orden.Detalles.ToList();
                ViewBag.NroOrden = orden.Numero;
                ViewBag.Usuario = usuario;
                ViewBag.Total = TruncarADosDecimales(orden.Total);
                return View(lista);
            }
            catch
            {
                TempData["Message"] = "Ocurrio un error al obtener detalle, Intentelo de nuevo mas tarde.";
                TempData["MessageType"] = "warning";
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public double TruncarADosDecimales(double? valor)
        {
            if (valor == null)
                return 0.0; // o lanza una excepción, según tu lógica

            return Math.Truncate(valor.Value * 100) / 100;
        }
    }
}
