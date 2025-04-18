namespace Minimarket.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
