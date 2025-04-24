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
            {
                return RedirectToAction("Login", "Account");
            }

            var total = await _context.Productos.CountAsync();
            ViewBag.TotalProductos = total;
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
