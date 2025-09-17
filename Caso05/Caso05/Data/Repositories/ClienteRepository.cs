using Microsoft.EntityFrameworkCore;
using Caso05.Data.Interfaces;
using Caso05.Models;

namespace Caso05.Data.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(Caso05DbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Cliente>> GetClientesConVentasAsync()
        {
            return await _dbSet
                .Include(c => c.Venta)
                .ToListAsync();
        }

        public async Task<Cliente?> GetClienteConVentasAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Detallesventa)
                        .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Clienteid == clienteId);
        }

        public async Task<IEnumerable<Cliente>> BuscarPorNombreOApellidoAsync(string texto)
        {
            return await _dbSet
                .Where(c => c.Nombre.ToLower().Contains(texto.ToLower()) || 
                           c.Apellido.ToLower().Contains(texto.ToLower()))
                .ToListAsync();
        }

        public override async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(c => c.Clienteid == id);
        }
    }
}