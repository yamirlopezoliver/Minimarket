using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;

namespace Minimarket.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ProyectoIntegradorContext _context;

        public AdminController(ProyectoIntegradorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsuarios = await _context.Users.CountAsync();
            ViewBag.TotalProductos = await _context.Productos.CountAsync();
            ViewBag.TotalOrdenes = await _context.Ordenes.CountAsync();

            var ventasSemanales = await _context.Ordenes
                .Where(o => o.FechaRecibida >= DateTime.Now.AddDays(-7))
                .GroupBy(o => o.FechaRecibida)
                .Select(g => new {
                    Dia = g.Key.ToString(),
                    Total = g.Count()
                })
                .ToListAsync();

            var dias = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var ventas = dias.Select(d => ventasSemanales.FirstOrDefault(v => v.Dia == d)?.Total ?? 0).ToArray();

            ViewBag.VentasPorDia = ventas;

            return View();
        }

    }
}
