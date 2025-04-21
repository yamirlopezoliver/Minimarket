using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Controllers
{
    public class AccountController : BaseController
    {

        private readonly ProyectoIntegradorContext _context;

        public AccountController(ProyectoIntegradorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password,
                    CreatedAt = DateTime.Now,
                    Tipo = "sin rol",
                    Direccion = model.Direccion,
                    Telefono = model.Telefono,
                    Nombre = model.Nombre
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                ViewData["SuccessMessage"] = "¡Felicidades, registro exitoso!";
                return View();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permisos)
                .SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user != null && !string.IsNullOrEmpty(user.Username))
            {
                int userRole = user.IdRol == 0 ? 0 : user.IdRol;
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetInt32("UserIdRol", user.IdRol);
                HttpContext.Session.SetString("Username", user.Username);

                var permisos = user.Role?.RolePermissions.Select(rp => rp.Permisos.Nombre).ToList();
                HttpContext.Session.SetString("Permisos", string.Join(",", permisos));

                if (permisos.Contains("VerProductos"))
                {
                    return RedirectToAction("Index", "Productos");
                }

                return RedirectToAction("Index", "Home");
            }
            // Mostrar mensaje de error si las credenciales son incorrectas
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("Permisos");

            HomeController.detalles.Clear();

            return RedirectToAction("Index", "Home");
        }


    }
}