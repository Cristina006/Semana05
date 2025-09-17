using Microsoft.EntityFrameworkCore.Storage;
using Caso05.Data.Interfaces;
using Caso05.Data.Repositories;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Caso05DbContext _context;
        private IDbContextTransaction? _transaction;

        private ICategoriaRepository? _categorias;
        private IClienteRepository? _clientes;
        private IProductoRepository? _productos;
        private IProveedorRepository? _proveedores;
        private IVentaRepository? _ventas;
        private IDetalleVentaRepository? _detallesVenta;
        private IMovimientoInventarioRepository? _movimientosInventario;

        public UnitOfWork(Caso05DbContext context)
        {
            _context = context;
        }

        public ICategoriaRepository Categorias =>
            _categorias ??= new CategoriaRepository(_context);

        public IClienteRepository Clientes =>
            _clientes ??= new ClienteRepository(_context);

        public IProductoRepository Productos =>
            _productos ??= new ProductoRepository(_context);

        public IProveedorRepository Proveedores =>
            _proveedores ??= new ProveedorRepository(_context);

        public IVentaRepository Ventas =>
            _ventas ??= new VentaRepository(_context);

        public IDetalleVentaRepository DetallesVenta =>
            _detallesVenta ??= new DetalleVentaRepository(_context);

        public IMovimientoInventarioRepository MovimientosInventario =>
            _movimientosInventario ??= new MovimientoInventarioRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}