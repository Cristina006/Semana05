using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Data.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MovimientosInventarioController> _logger;

        public MovimientosInventarioController(IUnitOfWork unitOfWork, ILogger<MovimientosInventarioController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimientosinventario>>> GetMovimientos()
        {
            try
            {
                var movimientos = await _unitOfWork.MovimientosInventario.GetMovimientosConProductosAsync();
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los movimientos de inventario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movimientosinventario>> GetMovimiento(int id)
        {
            try
            {
                var movimiento = await _unitOfWork.MovimientosInventario.GetByIdAsync(id);
                if (movimiento == null)
                {
                    return NotFound($"No se encontró el movimiento con ID {id}");
                }
                return Ok(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el movimiento con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("producto/{productId}")]
        public async Task<ActionResult<IEnumerable<Movimientosinventario>>> GetMovimientosPorProducto(int productId)
        {
            try
            {
                var movimientos = await _unitOfWork.MovimientosInventario.GetMovimientosPorProductoAsync(productId);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los movimientos del producto {ProductId}", productId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<Movimientosinventario>>> GetMovimientosPorTipo(string tipo)
        {
            try
            {
                var movimientos = await _unitOfWork.MovimientosInventario.GetMovimientosPorTipoAsync(tipo);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los movimientos por tipo {Tipo}", tipo);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("rango-fechas")]
        public async Task<ActionResult<IEnumerable<Movimientosinventario>>> GetMovimientosEnRangoFechas(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var movimientos = await _unitOfWork.MovimientosInventario.GetMovimientosEnRangoFechasAsync(fechaInicio, fechaFin);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los movimientos en el rango de fechas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("stock-actual/{productId}")]
        public async Task<ActionResult<int>> GetStockActual(int productId)
        {
            try
            {
                var stock = await _unitOfWork.MovimientosInventario.GetStockActualPorProductoAsync(productId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el stock actual del producto {ProductId}", productId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Movimientosinventario>> CreateMovimiento(Movimientosinventario movimiento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que el producto existe
                var producto = await _unitOfWork.Productos.GetByIdAsync(movimiento.Productoid);
                if (producto == null)
                {
                    return BadRequest($"No se encontró el producto con ID {movimiento.Productoid}");
                }

                // Validar tipo de movimiento
                var tiposValidos = new[] { "Entrada", "Salida" };
                if (!tiposValidos.Contains(movimiento.Tipomovimiento))
                {
                    return BadRequest("El tipo de movimiento debe ser 'Entrada' o 'Salida'");
                }

                movimiento.Fecha = DateTime.Now;
                var nuevoMovimiento = await _unitOfWork.MovimientosInventario.AddAsync(movimiento);

                // Actualizar stock del producto
                if (movimiento.Tipomovimiento.ToLower() == "entrada")
                {
                    producto.Stock += movimiento.Cantidad;
                }
                else if (movimiento.Tipomovimiento.ToLower() == "salida")
                {
                    if (producto.Stock < movimiento.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente. Stock actual: {producto.Stock}, cantidad solicitada: {movimiento.Cantidad}");
                    }
                    producto.Stock -= movimiento.Cantidad;
                }

                await _unitOfWork.Productos.UpdateAsync(producto);
                await _unitOfWork.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetMovimiento), new { id = nuevoMovimiento.Movimientoid }, nuevoMovimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el movimiento de inventario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            try
            {
                var movimiento = await _unitOfWork.MovimientosInventario.GetByIdAsync(id);
                if (movimiento == null)
                {
                    return NotFound($"No se encontró el movimiento con ID {id}");
                }

                // Revertir el efecto en el stock
                var producto = await _unitOfWork.Productos.GetByIdAsync(movimiento.Productoid);
                if (producto != null)
                {
                    if (movimiento.Tipomovimiento.ToLower() == "entrada")
                    {
                        producto.Stock -= movimiento.Cantidad;
                    }
                    else if (movimiento.Tipomovimiento.ToLower() == "salida")
                    {
                        producto.Stock += movimiento.Cantidad;
                    }
                    await _unitOfWork.Productos.UpdateAsync(producto);
                }

                await _unitOfWork.MovimientosInventario.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el movimiento con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}