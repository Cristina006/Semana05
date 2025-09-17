using Caso05.Data.Interfaces;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            return await _unitOfWork.Productos.GetProductosConCategoriasAsync();
        }

        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            return await _unitOfWork.Productos.GetProductoCompletoAsync(id);
        }

        public async Task<IEnumerable<Producto>> GetProductosConCategoriasAsync()
        {
            return await _unitOfWork.Productos.GetProductosConCategoriasAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosBajoStockAsync()
        {
            return await _unitOfWork.Productos.GetProductosBajoStockAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosPorCategoriaAsync(int categoriaId)
        {
            return await _unitOfWork.Productos.GetProductosPorCategoriaAsync(categoriaId);
        }

        public async Task<IEnumerable<Producto>> BuscarProductosPorNombreAsync(string nombre)
        {
            return await _unitOfWork.Productos.BuscarPorNombreAsync(nombre);
        }

        public async Task<Producto> CreateProductoAsync(Producto producto)
        {
            // Validar que la categoría existe si se proporciona
            if (producto.Categoriaid.HasValue)
            {
                var categoria = await _unitOfWork.Categorias.GetByIdAsync(producto.Categoriaid.Value);
                if (categoria == null)
                {
                    throw new InvalidOperationException($"No se encontró la categoría con ID {producto.Categoriaid}");
                }
            }

            // Validar que el proveedor existe si se proporciona
            if (producto.Proveedorid.HasValue)
            {
                var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(producto.Proveedorid.Value);
                if (proveedor == null)
                {
                    throw new InvalidOperationException($"No se encontró el proveedor con ID {producto.Proveedorid}");
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var nuevoProducto = await _unitOfWork.Productos.AddAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                // Registrar movimiento de inventario inicial si hay stock
                if (producto.Stock > 0)
                {
                    await RegistrarMovimientoInventarioAsync(nuevoProducto.Productoid, "Entrada", producto.Stock, null);
                }

                await _unitOfWork.CommitTransactionAsync();
                return nuevoProducto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            var productoExistente = await _unitOfWork.Productos.GetByIdAsync(producto.Productoid);
            if (productoExistente == null)
            {
                throw new InvalidOperationException($"No se encontró el producto con ID {producto.Productoid}");
            }

            await _unitOfWork.Productos.UpdateAsync(producto);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductoAsync(int id)
        {
            // Verificar que no tenga movimientos de inventario o ventas
            var detallesVenta = await _unitOfWork.DetallesVenta.GetDetallesPorProductoAsync(id);
            if (detallesVenta.Any())
            {
                throw new InvalidOperationException("No se puede eliminar el producto porque tiene ventas asociadas");
            }

            var movimientos = await _unitOfWork.MovimientosInventario.GetMovimientosPorProductoAsync(id);
            if (movimientos.Any())
            {
                throw new InvalidOperationException("No se puede eliminar el producto porque tiene movimientos de inventario");
            }

            await _unitOfWork.Productos.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarStockAsync(int productId, int nuevoStock)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(productId);
            if (producto == null)
            {
                throw new InvalidOperationException($"No se encontró el producto con ID {productId}");
            }

            int stockAnterior = producto.Stock;
            int diferencia = nuevoStock - stockAnterior;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Productos.ActualizarStockAsync(productId, nuevoStock);
                
                // Registrar el movimiento
                string tipoMovimiento = diferencia > 0 ? "Entrada" : "Salida";
                int cantidad = Math.Abs(diferencia);
                
                if (cantidad > 0)
                {
                    await RegistrarMovimientoInventarioAsync(productId, tipoMovimiento, cantidad, null);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RegistrarMovimientoInventarioAsync(int productId, string tipoMovimiento, int cantidad, int? referenciaId = null)
        {
            var movimiento = new Movimientosinventario
            {
                Productoid = productId,
                Fecha = DateTime.Now,
                Tipomovimiento = tipoMovimiento,
                Cantidad = cantidad,
                Referenciaid = referenciaId
            };

            await _unitOfWork.MovimientosInventario.AddAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}