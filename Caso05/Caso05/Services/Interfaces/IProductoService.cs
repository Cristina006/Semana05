using Caso05.Models;

namespace Caso05.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllProductosAsync();
        Task<Producto?> GetProductoByIdAsync(int id);
        Task<IEnumerable<Producto>> GetProductosConCategoriasAsync();
        Task<IEnumerable<Producto>> GetProductosBajoStockAsync();
        Task<IEnumerable<Producto>> GetProductosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Producto>> BuscarProductosPorNombreAsync(string nombre);
        Task<Producto> CreateProductoAsync(Producto producto);
        Task UpdateProductoAsync(Producto producto);
        Task DeleteProductoAsync(int id);
        Task ActualizarStockAsync(int productId, int nuevoStock);
        Task RegistrarMovimientoInventarioAsync(int productId, string tipoMovimiento, int cantidad, int? referenciaId = null);
    }
}