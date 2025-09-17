using System;
using System.Collections.Generic;

namespace Caso05.Models;

public partial class Proveedore
{
    public int Proveedorid { get; set; }

    public string Nombreempresa { get; set; } = null!;

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
