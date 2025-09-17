using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<Categoria?> GetByNombreAsync(string nombre);
        Task<IEnumerable<Categoria>> GetCategoriasConProductosAsync();
        Task<bool> TieneProductosAsync(int categoriaId);
    }
}