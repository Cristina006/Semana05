using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class MovimientoInventarioRepository : BaseRepository<Movimientosinventario>, IMovimientoInventarioRepository
    {
        public MovimientoInventarioRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Movimientosinventario>> GetMovimientosPorProductoAsync(int productId)
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(m => m.Productoid == productId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimientosinventario>> GetMovimientosPorTipoAsync(string tipoMovimiento)
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(m => m.Tipomovimiento.ToLower() == tipoMovimiento.ToLower())
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimientosinventario>> GetMovimientosPorFechaAsync(DateTime fecha)
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(m => m.Fecha.Date == fecha.Date)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimientosinventario>> GetMovimientosEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(m => m.Fecha.Date >= fechaInicio.Date && m.Fecha.Date <= fechaFin.Date)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimientosinventario>> GetMovimientosConProductosAsync()
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<int> GetStockActualPorProductoAsync(int productId)
        {
            var entradas = await _dbSet
                .Where(m => m.Productoid == productId && m.Tipomovimiento.ToLower() == "entrada")
                .SumAsync(m => m.Cantidad);

            var salidas = await _dbSet
                .Where(m => m.Productoid == productId && m.Tipomovimiento.ToLower() == "salida")
                .SumAsync(m => m.Cantidad);

            return entradas - salidas;
        }

        public override async Task<Movimientosinventario?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Movimientoid == id);
        }
    }
}