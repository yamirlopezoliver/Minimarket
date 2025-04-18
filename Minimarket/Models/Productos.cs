

namespace Minimarket.Models
{
    public class Productos
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Categoria { get; set; }
        
        public decimal Precio { get; set; }
        public int Stock { set; get; }
        public DateTime FechaIngreso{ get; set; }
        public string? Proveedor { get; set; }
    }
}
