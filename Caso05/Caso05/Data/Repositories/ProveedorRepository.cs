using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class ProveedorRepository : BaseRepository<Proveedore>, IProveedorRepository
    {
        public ProveedorRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<Proveedore?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Email != null && p.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Proveedore>> GetProveedoresConProductosAsync()
        {
            return await _dbSet
                .Include(p => p.Productos)
                .ToListAsync();
        }

        public async Task<Proveedore?> GetProveedorConProductosAsync(int proveedorId)
        {
            return await _dbSet
                .Include(p => p.Productos)
                    .ThenInclude(prod => prod.Categoria)
                .FirstOrDefaultAsync(p => p.Proveedorid == proveedorId);
        }

        public async Task<IEnumerable<Proveedore>> BuscarPorEmpresaAsync(string nombreEmpresa)
        {
            return await _dbSet
                .Where(p => p.Nombreempresa.ToLower().Contains(nombreEmpresa.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> TieneProductosAsync(int proveedorId)
        {
            return await _context.Productos
                .AnyAsync(p => p.Proveedorid == proveedorId);
        }

        public override async Task<Proveedore?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Productos)
                .FirstOrDefaultAsync(p => p.Proveedorid == id);
        }
    }
}