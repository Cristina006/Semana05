using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IDetalleVentaRepository : IRepository<Detallesventum>
    {
        Task<IEnumerable<Detallesventum>> GetDetallesPorVentaAsync(int ventaId);
        Task<IEnumerable<Detallesventum>> GetDetallesPorProductoAsync(int productId);
        Task<decimal> GetTotalVentaPorProductoAsync(int productId);
        Task<int> GetCantidadVendidaPorProductoAsync(int productId);
        Task<IEnumerable<Detallesventum>> GetDetallesConProductosAsync();
    }
}