using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IMovimientoInventarioRepository : IRepository<Movimientosinventario>
    {
        Task<IEnumerable<Movimientosinventario>> GetMovimientosPorProductoAsync(int productId);
        Task<IEnumerable<Movimientosinventario>> GetMovimientosPorTipoAsync(string tipoMovimiento);
        Task<IEnumerable<Movimientosinventario>> GetMovimientosPorFechaAsync(DateTime fecha);
        Task<IEnumerable<Movimientosinventario>> GetMovimientosEnRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Movimientosinventario>> GetMovimientosConProductosAsync();
        Task<int> GetStockActualPorProductoAsync(int productId);
    }
}