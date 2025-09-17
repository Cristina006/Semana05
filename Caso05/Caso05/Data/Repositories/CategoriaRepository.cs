using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<Categoria?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasConProductosAsync()
        {
            return await _dbSet
                .Include(c => c.Productos)
                .ToListAsync();
        }

        public async Task<bool> TieneProductosAsync(int categoriaId)
        {
            return await _context.Productos
                .AnyAsync(p => p.Categoriaid == categoriaId);
        }

        public override async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Categoriaid == id);
        }
    }
}