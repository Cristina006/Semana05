using System;
using System.Collections.Generic;

namespace Caso05.Models;

public partial class Venta
{
    public int Ventaid { get; set; }

    public int Clienteid { get; set; }

    public DateTime Fechaventa { get; set; }

    public decimal Total { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Detallesventum> Detallesventa { get; set; } = new List<Detallesventum>();
}
