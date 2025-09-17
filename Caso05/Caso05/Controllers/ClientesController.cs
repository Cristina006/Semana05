using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Data.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IUnitOfWork unitOfWork, ILogger<ClientesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            try
            {
                var clientes = await _unitOfWork.Clientes.GetClientesConVentasAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            try
            {
                var cliente = await _unitOfWork.Clientes.GetClienteConVentasAsync(id);
                if (cliente == null)
                {
                    return NotFound($"No se encontró el cliente con ID {id}");
                }
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("buscar/{texto}")]
        public async Task<ActionResult<IEnumerable<Cliente>>> BuscarClientes(string texto)
        {
            try
            {
                var clientes = await _unitOfWork.Clientes.BuscarPorNombreOApellidoAsync(texto);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes con texto {Texto}", texto);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CreateCliente(Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que no exista un cliente con el mismo email
                var clienteExistente = await _unitOfWork.Clientes.GetByEmailAsync(cliente.Email);
                if (clienteExistente != null)
                {
                    return BadRequest($"Ya existe un cliente con el email {cliente.Email}");
                }

                var nuevoCliente = await _unitOfWork.Clientes.AddAsync(cliente);
                await _unitOfWork.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.Clienteid }, nuevoCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, Cliente cliente)
        {
            try
            {
                if (id != cliente.Clienteid)
                {
                    return BadRequest("El ID del cliente no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clienteExistente = await _unitOfWork.Clientes.GetByIdAsync(id);
                if (clienteExistente == null)
                {
                    return NotFound($"No se encontró el cliente con ID {id}");
                }

                await _unitOfWork.Clientes.UpdateAsync(cliente);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var cliente = await _unitOfWork.Clientes.GetClienteConVentasAsync(id);
                if (cliente == null)
                {
                    return NotFound($"No se encontró el cliente con ID {id}");
                }

                if (cliente.Venta.Any())
                {
                    return BadRequest("No se puede eliminar el cliente porque tiene ventas asociadas");
                }

                await _unitOfWork.Clientes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}