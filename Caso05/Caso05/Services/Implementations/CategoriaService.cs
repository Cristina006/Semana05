using Caso05.Data.Interfaces;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Categoria>> GetAllCategoriasAsync()
        {
            return await _unitOfWork.Categorias.GetAllAsync();
        }

        public async Task<Categoria?> GetCategoriaByIdAsync(int id)
        {
            return await _unitOfWork.Categorias.GetByIdAsync(id);
        }

        public async Task<Categoria?> GetCategoriaByNombreAsync(string nombre)
        {
            return await _unitOfWork.Categorias.GetByNombreAsync(nombre);
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasConProductosAsync()
        {
            return await _unitOfWork.Categorias.GetCategoriasConProductosAsync();
        }

        public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
        {
            // Verificar si ya existe una categoría con el mismo nombre
            var existeCategoria = await _unitOfWork.Categorias.GetByNombreAsync(categoria.Nombre);
            if (existeCategoria != null)
            {
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{categoria.Nombre}'");
            }

            var nuevaCategoria = await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            return nuevaCategoria;
        }

        public async Task UpdateCategoriaAsync(Categoria categoria)
        {
            // Verificar si existe la categoría
            var categoriaExistente = await _unitOfWork.Categorias.GetByIdAsync(categoria.Categoriaid);
            if (categoriaExistente == null)
            {
                throw new InvalidOperationException($"No se encontró la categoría con ID {categoria.Categoriaid}");
            }

            // Verificar si el nuevo nombre no está siendo usado por otra categoría
            var categoriaConMismoNombre = await _unitOfWork.Categorias.GetByNombreAsync(categoria.Nombre);
            if (categoriaConMismoNombre != null && categoriaConMismoNombre.Categoriaid != categoria.Categoriaid)
            {
                throw new InvalidOperationException($"Ya existe otra categoría con el nombre '{categoria.Nombre}'");
            }

            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoriaAsync(int id)
        {
            // Verificar si la categoría tiene productos asociados
            var tieneProductos = await _unitOfWork.Categorias.TieneProductosAsync(id);
            if (tieneProductos)
            {
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados");
            }

            await _unitOfWork.Categorias.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> TieneProductosAsync(int categoriaId)
        {
            return await _unitOfWork.Categorias.TieneProductosAsync(categoriaId);
        }
    }
}