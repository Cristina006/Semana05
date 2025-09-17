using Caso05.Data.Interfaces;

namespace Caso05.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoriaRepository Categorias { get; }
        IClienteRepository Clientes { get; }
        IProductoRepository Productos { get; }
        IProveedorRepository Proveedores { get; }
        IVentaRepository Ventas { get; }
        IDetalleVentaRepository DetallesVenta { get; }
        IMovimientoInventarioRepository MovimientosInventario { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}