using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;

namespace Minimarket.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProyectoIntegradorContext _context;

        public ProductosController(ProyectoIntegradorContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string searchString, string category, DateTime? startDate, DateTime? endDate)
        {
            var productos = from p in _context.Productos
                            select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Nombre.StartsWith(searchString));
            }

            if (!String.IsNullOrEmpty(category))
            {
                productos = productos.Where(x => x.Categoria.StartsWith(category));
            }

            if (startDate.HasValue)
            {
                productos = productos.Where(x => x.FechaIngreso >= startDate);
            }

            if (endDate.HasValue)
            {
                productos = productos.Where(x => x.FechaIngreso <= endDate);
            }

            return View(await productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                    producto.UsuarioId = int.Parse(userId);

                if (producto.ImagenFile != null && producto.ImagenFile.Length > 0)
                {
                    string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);
                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenFile.FileName);
                    string ruta = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await producto.ImagenFile.CopyToAsync(stream);
                    }

                    producto.Imagen = nombreArchivo;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }


        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos.FindAsync(id);
            if (productos == null)
            {
                return NotFound();
            }
            return View(productos);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (producto.ImagenFile != null && producto.ImagenFile.Length > 0)
                {
                    string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);
                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenFile.FileName);
                    string ruta = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await producto.ImagenFile.CopyToAsync(stream);
                    }

                    producto.Imagen = nombreArchivo;
                }
                else if (producto.Imagen == null)
                {
                    var productoExistente = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (productoExistente != null)
                    {
                        producto.Imagen = productoExistente.Imagen;
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    producto.UsuarioId = int.Parse(userId);
                }

                _context.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }


        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productos = await _context.Productos.FindAsync(id);
            if (productos != null)
            {
                _context.Productos.Remove(productos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductosExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
