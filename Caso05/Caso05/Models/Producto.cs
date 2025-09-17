using System;
using System.Collections.Generic;

namespace Caso05.Models;

public partial class Producto
{
    public int Productoid { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public int Stockminimo { get; set; }

    public int? Categoriaid { get; set; }

    public int? Proveedorid { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual ICollection<Detallesventum> Detallesventa { get; set; } = new List<Detallesventum>();

    public virtual ICollection<Movimientosinventario> Movimientosinventarios { get; set; } = new List<Movimientosinventario>();

    public virtual Proveedore? Proveedor { get; set; }
}
