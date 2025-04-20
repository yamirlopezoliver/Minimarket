using System;
using System.Collections.Generic;

namespace Minimarket.Models;

public partial class Producto
{
    public int Id { get; set; }

    public int? Stock { get; set; }

    public string? Categoria { get; set; }

    public string? Descripcion { get; set; }

    public string? Imagen { get; set; }

    public string? Nombre { get; set; }

    public double? Precio { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int? UsuarioId { get; set; }

    public virtual ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();

    public virtual User? Usuario { get; set; }
}
