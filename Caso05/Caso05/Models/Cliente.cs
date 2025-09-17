using System;
using System.Collections.Generic;

namespace Caso05.Models;

public partial class Cliente
{
    public int Clienteid { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Direccionenvio { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
