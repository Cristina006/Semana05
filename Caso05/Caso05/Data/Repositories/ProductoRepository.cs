using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetProductosConCategoriasAsync()
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosConProveedoresAsync()
        {
            return await _dbSet
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        public async Task<Producto?> GetProductoCompletoAsync(int productId)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Include(p => p.Detallesventa)
                .Include(p => p.Movimientosinventarios)
                .FirstOrDefaultAsync(p => p.Productoid == productId);
        }

        public async Task<IEnumerable<Producto>> GetProductosPorCategoriaAsync(int categoriaId)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Categoriaid == categoriaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosPorProveedorAsync(int proveedorId)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Proveedorid == proveedorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosBajoStockAsync()
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Stock <= p.Stockminimo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
                .ToListAsync();
        }

        public async Task ActualizarStockAsync(int productId, int nuevoStock)
        {
            var producto = await _dbSet.FindAsync(productId);
            if (producto != null)
            {
                producto.Stock = nuevoStock;
            }
        }

        public override async Task<Producto?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Productoid == id);
        }
    }
}