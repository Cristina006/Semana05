using System;
using System.Collections.Generic;

namespace Caso05.Models;

public partial class Detallesventum
{
    public int Detalleventaid { get; set; }

    public int Ventaid { get; set; }

    public int Productoid { get; set; }

    public int Cantidad { get; set; }

    public decimal Preciounitario { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
