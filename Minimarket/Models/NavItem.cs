namespace Minimarket.Models
{
    public class NavItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string NombrePermisos { get; set; } = string.Empty;
    }
}
