using System;
using System.Collections.Generic;

namespace Minimarket.Models;

public partial class Detalle
{
    public int Id { get; set; }

    public int? Cantidad { get; set; }

    public string? Nombre { get; set; }

    public double? Precio { get; set; }

    public double? Total { get; set; }

    public int? OrdenId { get; set; }

    public int? ProductoId { get; set; }

    public virtual Ordene? Orden { get; set; }

    public virtual Producto? Producto { get; set; }
}
