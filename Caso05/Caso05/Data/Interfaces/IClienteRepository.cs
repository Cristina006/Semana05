using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> GetByEmailAsync(string email);
        Task<IEnumerable<Cliente>> GetClientesConVentasAsync();
        Task<Cliente?> GetClienteConVentasAsync(int clienteId);
        Task<IEnumerable<Cliente>> BuscarPorNombreOApellidoAsync(string texto);
    }
}