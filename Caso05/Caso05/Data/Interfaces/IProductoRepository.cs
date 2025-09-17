using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetProductosConCategoriasAsync();
        Task<IEnumerable<Producto>> GetProductosConProveedoresAsync();
        Task<Producto?> GetProductoCompletoAsync(int productId);
        Task<IEnumerable<Producto>> GetProductosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Producto>> GetProductosPorProveedorAsync(int proveedorId);
        Task<IEnumerable<Producto>> GetProductosBajoStockAsync();
        Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre);
        Task ActualizarStockAsync(int productId, int nuevoStock);
    }
}