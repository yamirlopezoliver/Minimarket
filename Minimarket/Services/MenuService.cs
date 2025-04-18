using Microsoft.EntityFrameworkCore;
using Minimarket.Data;
using Minimarket.Models;

namespace Minimarket.Services
{
    public class MenuService
    {
        private readonly ApplicationDbContext _context;

        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NavItem>> GetNavItemsForUserAsync(User user)
        {
            var permissionNames = _context.RolePermissions
                .Where(rp => rp.IdRol == user.IdRol)
                .Select(rp => rp.Permisos.Nombre);

            return await _context.NavItems
                .Where(nav => permissionNames.Contains(nav.NombrePermisos)).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        }

    }
}
