namespace Minimarket.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public Role Roles { get; set; } = default!;

        public int IdPermisos { get; set; }
        public Permission Permisos { get; set; } = default!;
    }
}
