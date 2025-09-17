using Caso05.Models;

namespace Caso05.Services.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<Venta>> GetAllVentasAsync();
        Task<Venta?> GetVentaByIdAsync(int id);
        Task<IEnumerable<Venta>> GetVentasPorClienteAsync(int clienteId);
        Task<IEnumerable<Venta>> GetVentasEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Venta> CreateVentaAsync(Venta venta, List<Detallesventum> detalles);
        Task UpdateVentaAsync(Venta venta);
        Task DeleteVentaAsync(int id);
        Task<decimal> GetTotalVentasPorClienteAsync(int clienteId);
        Task<decimal> GetTotalVentasPorFechaAsync(DateTime fecha);
    }
}