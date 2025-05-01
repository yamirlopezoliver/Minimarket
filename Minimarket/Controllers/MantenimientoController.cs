using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using System.Linq;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;

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

        public async Task<IActionResult> ReporteVentasDetallado()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ventasDetalladas = await _context.Ordenes
                .Include(o => o.Usuario)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return View("_ReporteVentasDetallado", ventasDetalladas);
        }
        // exportar en PDF
        public async Task<IActionResult> ExportarPdfReporteVentas()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ventasDetalladas = await _context.Ordenes
                .Include(o => o.Usuario)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Agregar título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Reporte de Ventas Detallado", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph(" ")); // Espacio

                // Crear tabla
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;
                // Anchos de columna ajustados para mayor precisión
                table.SetWidths(new float[] { 0.8f, 1.2f, 1.8f, 1.8f, 0.7f, 1.2f, 1.2f, 1.2f });

                // Agregar encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                table.AddCell(new PdfPCell(new Phrase("Nro. Orden", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Fecha Creación", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Usuario", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Producto", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Cant.", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", headerFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase("Total Detalle", headerFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase("Total Orden", headerFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                // Agregar datos
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                foreach (var orden in ventasDetalladas)
                {
                    foreach (var detalle in orden.Detalles)
                    {
                        table.AddCell(new PdfPCell(new Phrase(orden.Numero, cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(orden.FechaCreacion?.ToString("dd/MM/yyyy HH:mm") ?? "", cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(orden.Usuario?.Nombre + " (" + orden.Usuario?.Username + ")", cellFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        table.AddCell(new PdfPCell(new Phrase(detalle.Producto?.Nombre ?? "", cellFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        table.AddCell(new PdfPCell(new Phrase(detalle.Cantidad.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(detalle.Precio?.ToString("C") ?? "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(detalle.Total?.ToString("C") ?? "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                        // Manejo preciso de la celda con rowspan
                        if (orden.Detalles.First() == detalle)
                        {
                            PdfPCell totalOrdenCell = new PdfPCell(new Phrase(orden.Total?.ToString("C") ?? "", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                            {
                                Rowspan = orden.Detalles.Count,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(totalOrdenCell);
                        }
                        else
                        {
                            // No agregar celda adicional para las filas siguientes del mismo orden
                        }
                    }
                }

                document.Add(table);
                document.Close();

                byte[] pdfBytes = ms.ToArray();
                return File(pdfBytes, "application/pdf", "ReporteVentas.pdf");
            }
        }
        //FINAL PDF
    }
}