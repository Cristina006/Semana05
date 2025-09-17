using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class VentaRepository : BaseRepository<Venta>, IVentaRepository
    {
        public VentaRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Venta>> GetVentasConDetallesAsync()
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        public async Task<Venta?> GetVentaConDetallesAsync(int ventaId)
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(v => v.Ventaid == ventaId);
        }

        public async Task<IEnumerable<Venta>> GetVentasPorClienteAsync(int clienteId)
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Clienteid == clienteId)
                .OrderByDescending(v => v.Fechaventa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetVentasPorFechaAsync(DateTime fecha)
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Fechaventa.Date == fecha.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetVentasEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Fechaventa.Date >= fechaInicio.Date && v.Fechaventa.Date <= fechaFin.Date)
                .OrderByDescending(v => v.Fechaventa)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalVentasPorClienteAsync(int clienteId)
        {
            return await _dbSet
                .Where(v => v.Clienteid == clienteId)
                .SumAsync(v => v.Total);
        }

        public async Task<decimal> GetTotalVentasPorFechaAsync(DateTime fecha)
        {
            return await _dbSet
                .Where(v => v.Fechaventa.Date == fecha.Date)
                .SumAsync(v => v.Total);
        }

        public override async Task<Venta?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Detallesventa)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Ventaid == id);
        }
    }
}