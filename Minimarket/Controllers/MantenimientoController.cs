﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using System.Threading.Tasks;

namespace Minimarket.Controllers
{
    public class MantenimientoController : Controller
    {
        private readonly ProyectoIntegradorContext _context;

        public MantenimientoController(ProyectoIntegradorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Usuario)
                .ToListAsync();
            return View(productos);
        }
    }
}
