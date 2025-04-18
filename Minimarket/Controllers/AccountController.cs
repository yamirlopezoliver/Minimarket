using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Data;
using Minimarket.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Minimarket.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
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
                    CreatedAt = DateTime.Now
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
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // Redirigir a Productos/Index si las credenciales son correctas
                return RedirectToAction("Index", "Productos");
            }
            // Mostrar mensaje de error si las credenciales son incorrectas
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View();
        }

      

    }
}