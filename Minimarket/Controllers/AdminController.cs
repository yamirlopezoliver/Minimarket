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
            return View();
        }
    }
}
