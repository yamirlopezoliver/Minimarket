using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using System.Threading.Tasks;

namespace Minimarket.Controllers
{
    public class MantenimientoController : BaseController
    {
        private readonly ProyectoIntegradorContext _context;

        public MantenimientoController(ProyectoIntegradorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        public async Task<IActionResult> DashboardUsuarios()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var totalUsuarios = await _context.Users.CountAsync();

            var distribucion = await _context.Users
                .GroupBy(u => u.IdRol)
                .Select(g => new { RolId = g.Key, Count = g.Count() })
                .ToListAsync();

            var rolesMap = await _context.Roles
                .ToDictionaryAsync(r => r.IdRol, r => r.Nombre);

            var rolDistribucion = distribucion
                .Select(d => new {
                    Label = rolesMap.ContainsKey(d.RolId) ? rolesMap[d.RolId] : "Desconocido",
                    Count = d.Count
                })
                .ToList();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.RolDistribucion = rolDistribucion;
            ViewBag.RecentUsers = recentUsers;
            ViewBag.RolesLabels = rolDistribucion.Select(x => x.Label).ToList();
            ViewBag.RolesCounts = rolDistribucion.Select(x => x.Count).ToList();

            return PartialView("_DashboardUsuarios");
        }

        public async Task<IActionResult> DashboardProductos()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var totalProductos = await _context.Productos.CountAsync();

            var distribCategoria = await _context.Productos
                .GroupBy(p => p.Categoria ?? "Sin categoría")
                .Select(g => new { Categoria = g.Key, Count = g.Count() })
                .ToListAsync();

            var ventasPorProducto = await _context.Productos
                .GroupJoin(
                    _context.Detalles,
                    p => p.Id,
                    d => d.ProductoId,
                    (p, detallesGroup) => new {
                        Nombre = p.Nombre,
                        TotalVendido = detallesGroup.Sum(d => (int?)d.Cantidad) ?? 0
                    }
                )
                .OrderByDescending(x => x.TotalVendido)
                .ToListAsync();

            var vendidos = ventasPorProducto.Where(v => v.TotalVendido > 0).ToList();
            var totalUnidades = vendidos.Sum(v => v.TotalVendido);
            var maxVendido = vendidos.Any() ? vendidos.Max(v => v.TotalVendido) : 0;
            var topNames = vendidos.Where(v => v.TotalVendido == maxVendido)
                                         .Select(v => v.Nombre).ToList();
            var sinVentas = await _context.Productos.CountAsync() - vendidos.Count;

            int umbral = 5;
            var lowStockCount = await _context.Productos.CountAsync(p => p.Stock <= umbral);

            var totalIngresos = await _context.Detalles
                .SumAsync(d => d.Cantidad * d.Precio);

            var ultimas5 = await _context.Ordenes
                .OrderByDescending(o => o.FechaCreacion)
                .Take(5)
                .SelectMany(o => o.Detalles.Select(d => new {
                    Fecha = o.FechaCreacion,
                    Producto = d.Nombre,
                    Cantidad = d.Cantidad,
                    Total = d.Total
                }))
                .ToListAsync();

            ViewBag.TotalProductos = totalProductos;
            ViewBag.CatLabels = distribCategoria.Select(x => x.Categoria).ToList();
            ViewBag.CatCounts = distribCategoria.Select(x => x.Count).ToList();
            ViewBag.VentasLabels = vendidos.Select(v => v.Nombre).ToList();
            ViewBag.VentasCounts = vendidos.Select(v => v.TotalVendido).ToList();
            ViewBag.TopNames = topNames;
            ViewBag.TopCount = maxVendido;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.Ultimas5 = ultimas5;

            return PartialView("_DashboardProductos");
        }


        public async Task<IActionResult> DashboardOrdenes()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var total = await _context.Ordenes.CountAsync();
            ViewBag.TotalOrdenes = total;
            return PartialView("_DashboardOrdenes");
        }


        public async Task<IActionResult> Productos()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var productos = await _context.Productos
                .Include(p => p.Usuario)
                .ToListAsync();
            return View(productos);
        }

        public async Task<IActionResult> Usuarios()
        {
            var users = await _context.Users
               .Include(u => u.Role)
               .ToListAsync();
            return View(users);
        }
    }
}
