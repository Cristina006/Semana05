using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IVentaRepository : IRepository<Venta>
    {
        Task<IEnumerable<Venta>> GetVentasConDetallesAsync();
        Task<Venta?> GetVentaConDetallesAsync(int ventaId);
        Task<IEnumerable<Venta>> GetVentasPorClienteAsync(int clienteId);
        Task<IEnumerable<Venta>> GetVentasPorFechaAsync(DateTime fecha);
        Task<IEnumerable<Venta>> GetVentasEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<decimal> GetTotalVentasPorClienteAsync(int clienteId);
        Task<decimal> GetTotalVentasPorFechaAsync(DateTime fecha);
    }
}