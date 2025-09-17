using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly ILogger<VentasController> _logger;

        public VentasController(IVentaService ventaService, ILogger<VentasController> logger)
        {
            _ventaService = ventaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            try
            {
                var ventas = await _ventaService.GetAllVentasAsync();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ventas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            try
            {
                var venta = await _ventaService.GetVentaByIdAsync(id);
                if (venta == null)
                {
                    return NotFound($"No se encontró la venta con ID {id}");
                }
                return Ok(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la venta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentasPorCliente(int clienteId)
        {
            try
            {
                var ventas = await _ventaService.GetVentasPorClienteAsync(clienteId);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ventas del cliente {ClienteId}", clienteId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("rango-fechas")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentasEnRangoFechas(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var ventas = await _ventaService.GetVentasEnRangoFechasAsync(fechaInicio, fechaFin);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ventas en el rango de fechas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Venta>> CreateVenta([FromBody] CreateVentaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nuevaVenta = await _ventaService.CreateVentaAsync(request.Venta, request.Detalles);
                return CreatedAtAction(nameof(GetVenta), new { id = nuevaVenta.Ventaid }, nuevaVenta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la venta");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenta(int id, Venta venta)
        {
            try
            {
                if (id != venta.Ventaid)
                {
                    return BadRequest("El ID de la venta no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _ventaService.UpdateVentaAsync(venta);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la venta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            try
            {
                await _ventaService.DeleteVentaAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la venta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("total-cliente/{clienteId}")]
        public async Task<ActionResult<decimal>> GetTotalVentasPorCliente(int clienteId)
        {
            try
            {
                var total = await _ventaService.GetTotalVentasPorClienteAsync(clienteId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el total de ventas del cliente {ClienteId}", clienteId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("total-fecha")]
        public async Task<ActionResult<decimal>> GetTotalVentasPorFecha([FromQuery] DateTime fecha)
        {
            try
            {
                var total = await _ventaService.GetTotalVentasPorFechaAsync(fecha);
                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el total de ventas por fecha");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    public class CreateVentaRequest
    {
        public Venta Venta { get; set; } = null!;
        public List<Detallesventum> Detalles { get; set; } = new();
    }
}