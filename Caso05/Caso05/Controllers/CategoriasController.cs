using Microsoft.AspNetCore.Mvc;
using Caso05.Models;
using Caso05.Services.Interfaces;

namespace Caso05.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            try
            {
                var categorias = await _categoriaService.GetAllCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetCategoriaByIdAsync(id);
                if (categoria == null)
                {
                    return NotFound($"No se encontró la categoría con ID {id}");
                }
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("con-productos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasConProductos()
        {
            try
            {
                var categorias = await _categoriaService.GetCategoriasConProductosAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías con productos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("buscar/{nombre}")]
        public async Task<ActionResult<Categoria>> GetCategoriaPorNombre(string nombre)
        {
            try
            {
                var categoria = await _categoriaService.GetCategoriaByNombreAsync(nombre);
                if (categoria == null)
                {
                    return NotFound($"No se encontró la categoría con nombre '{nombre}'");
                }
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar la categoría con nombre {Nombre}", nombre);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> CreateCategoria(Categoria categoria)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nuevaCategoria = await _categoriaService.CreateCategoriaAsync(categoria);
                return CreatedAtAction(nameof(GetCategoria), new { id = nuevaCategoria.Categoriaid }, nuevaCategoria);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, Categoria categoria)
        {
            try
            {
                if (id != categoria.Categoriaid)
                {
                    return BadRequest("El ID de la categoría no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _categoriaService.UpdateCategoriaAsync(categoria);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                await _categoriaService.DeleteCategoriaAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}/tiene-productos")]
        public async Task<ActionResult<bool>> TieneProductos(int id)
        {
            try
            {
                var tieneProductos = await _categoriaService.TieneProductosAsync(id);
                return Ok(tieneProductos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si la categoría {Id} tiene productos", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}