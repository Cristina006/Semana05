using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class DetalleVentaRepository : BaseRepository<Detallesventum>, IDetalleVentaRepository
    {
        public DetalleVentaRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Detallesventum>> GetDetallesPorVentaAsync(int ventaId)
        {
            return await _dbSet
                .Include(d => d.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(d => d.Venta)
                .Where(d => d.Ventaid == ventaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Detallesventum>> GetDetallesPorProductoAsync(int productId)
        {
            return await _dbSet
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                    .ThenInclude(v => v.Cliente)
                .Where(d => d.Productoid == productId)
                .OrderByDescending(d => d.Venta.Fechaventa)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalVentaPorProductoAsync(int productId)
        {
            return await _dbSet
                .Where(d => d.Productoid == productId)
                .SumAsync(d => d.Cantidad * d.Preciounitario);
        }

        public async Task<int> GetCantidadVendidaPorProductoAsync(int productId)
        {
            return await _dbSet
                .Where(d => d.Productoid == productId)
                .SumAsync(d => d.Cantidad);
        }

        public async Task<IEnumerable<Detallesventum>> GetDetallesConProductosAsync()
        {
            return await _dbSet
                .Include(d => d.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(d => d.Venta)
                    .ThenInclude(v => v.Cliente)
                .ToListAsync();
        }

        public override async Task<Detallesventum?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(d => d.Venta)
                    .ThenInclude(v => v.Cliente)
                .FirstOrDefaultAsync(d => d.Detalleventaid == id);
        }
    }
}