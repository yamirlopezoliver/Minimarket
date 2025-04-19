using System;
using System.Collections.Generic;

namespace Minimarket.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Direccion { get; set; }

    public string? Email { get; set; }

    public string? Nombre { get; set; }

    public string? Password { get; set; }

    public string? Telefono { get; set; }

    public string? Tipo { get; set; }

    public string? Username { get; set; }

    public DateTime? CreatedAt { get; set; }
    public int IdRol { get; set; }
    public Role? Role { get; set; }

    public virtual ICollection<Ordene> Ordenes { get; set; } = new List<Ordene>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
