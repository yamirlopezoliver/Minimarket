namespace Minimarket.Models
{
    public class Role
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
