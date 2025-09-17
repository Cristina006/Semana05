using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Data.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(IUnitOfWork unitOfWork, ILogger<ProveedoresController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedore>>> GetProveedores()
        {
            try
            {
                var proveedores = await _unitOfWork.Proveedores.GetProveedoresConProductosAsync();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los proveedores");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedore>> GetProveedor(int id)
        {
            try
            {
                var proveedor = await _unitOfWork.Proveedores.GetProveedorConProductosAsync(id);
                if (proveedor == null)
                {
                    return NotFound($"No se encontró el proveedor con ID {id}");
                }
                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proveedor con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("buscar/{empresa}")]
        public async Task<ActionResult<IEnumerable<Proveedore>>> BuscarProveedores(string empresa)
        {
            try
            {
                var proveedores = await _unitOfWork.Proveedores.BuscarPorEmpresaAsync(empresa);
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proveedores con empresa {Empresa}", empresa);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Proveedore>> CreateProveedor(Proveedore proveedor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que no exista un proveedor con el mismo email si se proporciona
                if (!string.IsNullOrEmpty(proveedor.Email))
                {
                    var proveedorExistente = await _unitOfWork.Proveedores.GetByEmailAsync(proveedor.Email);
                    if (proveedorExistente != null)
                    {
                        return BadRequest($"Ya existe un proveedor con el email {proveedor.Email}");
                    }
                }

                var nuevoProveedor = await _unitOfWork.Proveedores.AddAsync(proveedor);
                await _unitOfWork.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetProveedor), new { id = nuevoProveedor.Proveedorid }, nuevoProveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el proveedor");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProveedor(int id, Proveedore proveedor)
        {
            try
            {
                if (id != proveedor.Proveedorid)
                {
                    return BadRequest("El ID del proveedor no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var proveedorExistente = await _unitOfWork.Proveedores.GetByIdAsync(id);
                if (proveedorExistente == null)
                {
                    return NotFound($"No se encontró el proveedor con ID {id}");
                }

                await _unitOfWork.Proveedores.UpdateAsync(proveedor);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el proveedor con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            try
            {
                var tieneProductos = await _unitOfWork.Proveedores.TieneProductosAsync(id);
                if (tieneProductos)
                {
                    return BadRequest("No se puede eliminar el proveedor porque tiene productos asociados");
                }

                await _unitOfWork.Proveedores.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el proveedor con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}/tiene-productos")]
        public async Task<ActionResult<bool>> TieneProductos(int id)
        {
            try
            {
                var tieneProductos = await _unitOfWork.Proveedores.TieneProductosAsync(id);
                return Ok(tieneProductos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el proveedor {Id} tiene productos", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}