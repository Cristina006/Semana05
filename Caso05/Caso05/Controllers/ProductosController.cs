using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            try
            {
                var productos = await _productoService.GetAllProductosAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                if (producto == null)
                {
                    return NotFound($"No se encontró el producto con ID {id}");
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("bajo-stock")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosBajoStock()
        {
            try
            {
                var productos = await _productoService.GetProductosBajoStockAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos bajo stock");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorCategoria(int categoriaId)
        {
            try
            {
                var productos = await _productoService.GetProductosPorCategoriaAsync(categoriaId);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos por categoría {CategoriaId}", categoriaId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("buscar/{nombre}")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos(string nombre)
        {
            try
            {
                var productos = await _productoService.BuscarProductosPorNombreAsync(nombre);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con nombre {Nombre}", nombre);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> CreateProducto(Producto producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nuevoProducto = await _productoService.CreateProductoAsync(producto);
                return CreatedAtAction(nameof(GetProducto), new { id = nuevoProducto.Productoid }, nuevoProducto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, Producto producto)
        {
            try
            {
                if (id != producto.Productoid)
                {
                    return BadRequest("El ID del producto no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _productoService.UpdateProductoAsync(producto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> ActualizarStock(int id, [FromBody] int nuevoStock)
        {
            try
            {
                await _productoService.ActualizarStockAsync(id, nuevoStock);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el stock del producto con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                await _productoService.DeleteProductoAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}