using Caso05.Data.Interfaces;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Venta>> GetAllVentasAsync()
        {
            return await _unitOfWork.Ventas.GetVentasConDetallesAsync();
        }

        public async Task<Venta?> GetVentaByIdAsync(int id)
        {
            return await _unitOfWork.Ventas.GetVentaConDetallesAsync(id);
        }

        public async Task<IEnumerable<Venta>> GetVentasPorClienteAsync(int clienteId)
        {
            return await _unitOfWork.Ventas.GetVentasPorClienteAsync(clienteId);
        }

        public async Task<IEnumerable<Venta>> GetVentasEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _unitOfWork.Ventas.GetVentasEnRangoFechasAsync(fechaInicio, fechaFin);
        }

        public async Task<Venta> CreateVentaAsync(Venta venta, List<Detallesventum> detalles)
        {
            // Validar que el cliente existe
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(venta.Clienteid);
            if (cliente == null)
            {
                throw new InvalidOperationException($"No se encontró el cliente con ID {venta.Clienteid}");
            }

            // Validar que hay detalles
            if (!detalles.Any())
            {
                throw new InvalidOperationException("La venta debe tener al menos un detalle");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validar stock y calcular total
                decimal totalVenta = 0;
                foreach (var detalle in detalles)
                {
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.Productoid);
                    if (producto == null)
                    {
                        throw new InvalidOperationException($"No se encontró el producto con ID {detalle.Productoid}");
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}. Stock disponible: {producto.Stock}, solicitado: {detalle.Cantidad}");
                    }

                    totalVenta += detalle.Cantidad * detalle.Preciounitario;
                }

                // Crear la venta
                venta.Total = totalVenta;
                venta.Fechaventa = DateTime.Now;
                var nuevaVenta = await _unitOfWork.Ventas.AddAsync(venta);
                await _unitOfWork.SaveChangesAsync();

                // Crear los detalles y actualizar stock
                foreach (var detalle in detalles)
                {
                    detalle.Ventaid = nuevaVenta.Ventaid;
                    await _unitOfWork.DetallesVenta.AddAsync(detalle);

                    // Actualizar stock del producto
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.Productoid);
                    producto!.Stock -= detalle.Cantidad;
                    await _unitOfWork.Productos.UpdateAsync(producto);

                    // Registrar movimiento de inventario
                    var movimiento = new Movimientosinventario
                    {
                        Productoid = detalle.Productoid,
                        Fecha = DateTime.Now,
                        Tipomovimiento = "Salida",
                        Cantidad = detalle.Cantidad,
                        Referenciaid = nuevaVenta.Ventaid
                    };
                    await _unitOfWork.MovimientosInventario.AddAsync(movimiento);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return nuevaVenta;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateVentaAsync(Venta venta)
        {
            var ventaExistente = await _unitOfWork.Ventas.GetByIdAsync(venta.Ventaid);
            if (ventaExistente == null)
            {
                throw new InvalidOperationException($"No se encontró la venta con ID {venta.Ventaid}");
            }

            await _unitOfWork.Ventas.UpdateAsync(venta);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteVentaAsync(int id)
        {
            var venta = await _unitOfWork.Ventas.GetVentaConDetallesAsync(id);
            if (venta == null)
            {
                throw new InvalidOperationException($"No se encontró la venta con ID {id}");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Restaurar stock de los productos
                foreach (var detalle in venta.Detallesventa)
                {
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.Productoid);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                        await _unitOfWork.Productos.UpdateAsync(producto);

                        // Registrar movimiento de inventario de devolución
                        var movimiento = new Movimientosinventario
                        {
                            Productoid = detalle.Productoid,
                            Fecha = DateTime.Now,
                            Tipomovimiento = "Entrada",
                            Cantidad = detalle.Cantidad,
                            Referenciaid = id
                        };
                        await _unitOfWork.MovimientosInventario.AddAsync(movimiento);
                    }

                    // Eliminar detalle
                    await _unitOfWork.DetallesVenta.DeleteAsync(detalle.Detalleventaid);
                }

                // Eliminar venta
                await _unitOfWork.Ventas.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<decimal> GetTotalVentasPorClienteAsync(int clienteId)
        {
            return await _unitOfWork.Ventas.GetTotalVentasPorClienteAsync(clienteId);
        }

        public async Task<decimal> GetTotalVentasPorFechaAsync(DateTime fecha)
        {
            return await _unitOfWork.Ventas.GetTotalVentasPorFechaAsync(fecha);
        }
    }
}