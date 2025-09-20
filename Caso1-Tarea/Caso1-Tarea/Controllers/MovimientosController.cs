using Microsoft.AspNetCore.Mvc;
using Caso1_Tarea.DTOs;
using Caso1_Tarea.Models;
using Microsoft.EntityFrameworkCore;
using Caso1_Tarea.Models.Enums;
using Caso1_Tarea.Repositories;

namespace Caso1_Tarea.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MovimientosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovimientosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todos los movimientos de inventario
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovimientoInventarioDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetMovimientos()
        {
            var movimientos = await _unitOfWork.MovimientosInventario.GetQueryable()
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            var movimientosDto = movimientos.Select(m => new MovimientoInventarioDto
            {
                Movimientoid = m.Movimientoid,
                Productoid = m.Productoid,
                Tipomovimiento = m.Tipomovimiento,
                Cantidad = m.Cantidad,
                Fecha = m.Fecha,
                NombreProducto = m.Producto?.Nombre 
            });

            return Ok(movimientosDto);
        }

        /// <summary>
        /// Registra un nuevo movimiento de inventario (Entrada o Salida)
        /// </summary>
        /// <remarks>
        /// Este endpoint es transaccional. Actualiza el stock del producto automáticamente.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(MovimientoInventarioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovimientoInventarioDto>> CreateMovimiento(CreateMovimientoInventarioDto createDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validar que el producto exista
                var producto = await _unitOfWork.Productos.GetByIdAsync(createDto.Productoid);
                if (producto == null)
                {
                    return NotFound($"Producto con ID {createDto.Productoid} no encontrado.");
                }

                // Crear el nuevo movimiento
                var movimiento = new Movimientosinventario
                {
                    Productoid = createDto.Productoid,
                    Tipomovimiento = createDto.Tipomovimiento,
                    Cantidad = createDto.Cantidad,
                    Fecha = DateTime.UtcNow 
                };

                // Actualizar el stock del producto
                if (movimiento.Tipomovimiento == TipoMovimiento.Entrada)
                {
                    producto.Cantidadinventario += movimiento.Cantidad;
                }
                else if (movimiento.Tipomovimiento == TipoMovimiento.Salida)
                {
                    // Validar si hay stock suficiente para la salida
                    if (producto.Cantidadinventario < movimiento.Cantidad)
                    {
                        await _unitOfWork.RollbackAsync(); 
                        return BadRequest($"Stock insuficiente para el producto '{producto.Nombre}'. Stock actual: {producto.Cantidadinventario}.");
                    }
                    producto.Cantidadinventario -= movimiento.Cantidad;
                }

                // Guardar los cambios en la base de datos
                await _unitOfWork.MovimientosInventario.AddAsync(movimiento);
                _unitOfWork.Productos.Update(producto);
                await _unitOfWork.SaveChangesAsync();

                // Si todo fue exitoso, confirmar la transacción
                await _unitOfWork.CommitAsync();

                // Mapear a DTO para la respuesta
                var movimientoDto = new MovimientoInventarioDto
                {
                    Movimientoid = movimiento.Movimientoid,
                    Productoid = movimiento.Productoid,
                    Tipomovimiento = movimiento.Tipomovimiento,
                    Cantidad = movimiento.Cantidad,
                    Fecha = movimiento.Fecha,
                    NombreProducto = producto.Nombre
                };
                
                return CreatedAtAction(nameof(GetMovimientoById), new { id = movimiento.Movimientoid }, movimientoDto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un movimiento de inventario por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovimientoInventarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovimientoInventarioDto>> GetMovimientoById(int id)
        {
            var movimiento = await _unitOfWork.MovimientosInventario.GetQueryable()
                .Include(m => m.Producto)
                .FirstOrDefaultAsync(m => m.Movimientoid == id);

            if (movimiento == null)
            {
                return NotFound($"Movimiento con ID {id} no encontrado.");
            }

            var movimientoDto = new MovimientoInventarioDto
            {
                Movimientoid = movimiento.Movimientoid,
                Productoid = movimiento.Productoid,
                Tipomovimiento = movimiento.Tipomovimiento,
                Cantidad = movimiento.Cantidad,
                Fecha = movimiento.Fecha,
                NombreProducto = movimiento.Producto?.Nombre
            };

            return Ok(movimientoDto);
        }
    }
}

