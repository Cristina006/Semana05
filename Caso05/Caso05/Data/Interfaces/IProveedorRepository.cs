using Caso05.Models;

namespace Caso05.Data.Interfaces
{
    public interface IProveedorRepository : IRepository<Proveedore>
    {
        Task<Proveedore?> GetByEmailAsync(string email);
        Task<IEnumerable<Proveedore>> GetProveedoresConProductosAsync();
        Task<Proveedore?> GetProveedorConProductosAsync(int proveedorId);
        Task<IEnumerable<Proveedore>> BuscarPorEmpresaAsync(string nombreEmpresa);
        Task<bool> TieneProductosAsync(int proveedorId);
    }
}