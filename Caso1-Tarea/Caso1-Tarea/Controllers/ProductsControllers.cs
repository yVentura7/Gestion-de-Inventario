using Microsoft.AspNetCore.Mvc;
using Caso1_Tarea.DTOs;
using Caso1_Tarea.Models;
using Caso1_Tarea.Repositories;
using Caso1_Tarea.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Caso1_Tarea.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _unitOfWork.Productos.GetAllAsync();
            var productosDto = productos.Select(p => new ProductoDto
            {
                Productoid = p.Productoid,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Categoria = p.Categoria,
                Cantidadinventario = p.Cantidadinventario,
                Stockminimo = p.Stockminimo
            });

            return Ok(productosDto);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            var productoDto = new ProductoDto
            {
                Productoid = producto.Productoid,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Categoria = producto.Categoria,
                Cantidadinventario = producto.Cantidadinventario,
                Stockminimo = producto.Stockminimo
            };

            return Ok(productoDto);
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> CreateProducto(CreateProductoDto createDto)
        {
            var producto = new Producto
            {
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                Precio = createDto.Precio,
                Categoria = createDto.Categoria,
                Cantidadinventario = createDto.Cantidadinventario,
                Stockminimo = createDto.Stockminimo
            };

            await _unitOfWork.Productos.AddAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            var productoDto = new ProductoDto
            {
                Productoid = producto.Productoid,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Categoria = producto.Categoria,
                Cantidadinventario = producto.Cantidadinventario,
                Stockminimo = producto.Stockminimo
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Productoid }, productoDto);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProducto(int id, UpdateProductoDto updateDto)
        {
            if (id != updateDto.Productoid)
            {
                return BadRequest("El ID no coincide");
            }

            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            producto.Nombre = updateDto.Nombre;
            producto.Descripcion = updateDto.Descripcion;
            producto.Precio = updateDto.Precio;
            producto.Categoria = updateDto.Categoria;
            producto.Cantidadinventario = updateDto.Cantidadinventario;
            producto.Stockminimo = updateDto.Stockminimo;

            _unitOfWork.Productos.Update(producto);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            _unitOfWork.Productos.Remove(producto);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene productos con stock bajo (menor al mínimo)
        /// </summary>
        [HttpGet("stock-bajo")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductosStockBajo()
        {
            var productos = await _unitOfWork.Productos
                .FindAsync(p => p.Cantidadinventario < (p.Stockminimo ?? 10));

            var productosDto = productos.Select(p => new ProductoDto
            {
                Productoid = p.Productoid,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Categoria = p.Categoria,
                Cantidadinventario = p.Cantidadinventario,
                Stockminimo = p.Stockminimo
            });

            return Ok(productosDto);
        }
    }
}