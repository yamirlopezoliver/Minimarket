using System;
using System.Collections.Generic;

namespace Minimarket.Models;

public partial class Ordene
{
    public int Id { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaRecibida { get; set; }

    public string? Numero { get; set; }

    public double? Total { get; set; }

    public int? UsuarioId { get; set; }

    public virtual ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();

    public virtual User? Usuario { get; set; }
}
