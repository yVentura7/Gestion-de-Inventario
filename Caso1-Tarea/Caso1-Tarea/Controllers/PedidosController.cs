using Microsoft.AspNetCore.Mvc;
using Caso1_Tarea.DTOs;
using Caso1_Tarea.Models;
using Caso1_Tarea.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Caso1_Tarea.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PedidosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PedidosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todos los pedidos con sus detalles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PedidoProveedorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoProveedorDto>>> GetPedidos()
        {
            var pedidos = await _unitOfWork.PedidosProveedores.GetQueryable()
                .Include(p => p.Proveedor)
                .Include(p => p.Detallepedidos)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            var pedidosDto = pedidos.Select(p => new PedidoProveedorDto
            {
                Pedidoid = p.Pedidoid,
                Proveedorid = p.Proveedorid,
                Fechapedido = p.Fechapedido,
                Estado = p.Estado,
                NombreProveedor = p.Proveedor?.Nombre,
                Detalles = p.Detallepedidos.Select(d => new DetallePedidoDto
                {
                    Detalleid = d.Detalleid,
                    Pedidoid = d.Pedidoid,
                    Productoid = d.Productoid,
                    Cantidad = d.Cantidad,
                    NombreProducto = d.Producto?.Nombre,
                    PrecioUnitario = d.Producto?.Precio
                }).ToList()
            });

            return Ok(pedidosDto);
        }
        
        /// <summary>
        /// Obtiene un pedido por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PedidoProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoProveedorDto>> GetPedido(int id)
        {
            var pedido = await _unitOfWork.PedidosProveedores.GetQueryable()
                .Include(p => p.Proveedor)
                .Include(p => p.Detallepedidos)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Pedidoid == id);

            if (pedido == null)
            {
                return NotFound($"Pedido con ID {id} no encontrado.");
            }
            
            var pedidoDto = new PedidoProveedorDto
            {
                Pedidoid = pedido.Pedidoid,
                Proveedorid = pedido.Proveedorid,
                Fechapedido = pedido.Fechapedido,
                Estado = pedido.Estado,
                NombreProveedor = pedido.Proveedor?.Nombre,
                Detalles = pedido.Detallepedidos.Select(d => new DetallePedidoDto
                {
                    Detalleid = d.Detalleid,
                    Pedidoid = d.Pedidoid,
                    Productoid = d.Productoid,
                    Cantidad = d.Cantidad,
                    NombreProducto = d.Producto?.Nombre,
                    PrecioUnitario = d.Producto?.Precio
                }).ToList()
            };

            return Ok(pedidoDto);
        }


        /// <summary>
        /// Crea un nuevo pedido con sus detalles
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PedidoProveedorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PedidoProveedorDto>> CreatePedido(CreatePedidoProveedorDto createDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                var pedido = new Pedidosproveedore
                {
                    Proveedorid = createDto.Proveedorid,
                    Fechapedido = createDto.Fechapedido,
                    Estado = createDto.Estado
                };

                await _unitOfWork.PedidosProveedores.AddAsync(pedido);
                await _unitOfWork.SaveChangesAsync(); 

                foreach (var detalleDto in createDto.Detalles)
                {
                    var detalle = new Detallepedido
                    {
                        Pedidoid = pedido.Pedidoid, 
                        Productoid = detalleDto.Productoid,
                        Cantidad = detalleDto.Cantidad
                    };

                    await _unitOfWork.DetallePedidos.AddAsync(detalle);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                
                var pedidoCreadoDto = await GetPedido(pedido.Pedidoid);
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Pedidoid }, pedidoCreadoDto.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el estado de un pedido
        /// </summary>
        [HttpPut("{id}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEstadoPedido(int id, [FromBody] UpdatePedidoProveedorDto updateDto)
        {
            var pedido = await _unitOfWork.PedidosProveedores.GetByIdAsync(id);
            if (pedido == null)
            {
                return NotFound($"Pedido con ID {id} no encontrado");
            }

            pedido.Estado = updateDto.Estado;
            _unitOfWork.PedidosProveedores.Update(pedido);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina un pedido y sus detalles
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _unitOfWork.PedidosProveedores.GetByIdAsync(id);
                
            if (pedido == null)
            {
                return NotFound($"Pedido con ID {id} no encontrado");
            }

            _unitOfWork.PedidosProveedores.Remove(pedido);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
