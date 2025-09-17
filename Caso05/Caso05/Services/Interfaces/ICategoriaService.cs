using Caso05.Models;

namespace Caso05.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetAllCategoriasAsync();
        Task<Categoria?> GetCategoriaByIdAsync(int id);
        Task<Categoria?> GetCategoriaByNombreAsync(string nombre);
        Task<IEnumerable<Categoria>> GetCategoriasConProductosAsync();
        Task<Categoria> CreateCategoriaAsync(Categoria categoria);
        Task UpdateCategoriaAsync(Categoria categoria);
        Task DeleteCategoriaAsync(int id);
        Task<bool> TieneProductosAsync(int categoriaId);
    }
}