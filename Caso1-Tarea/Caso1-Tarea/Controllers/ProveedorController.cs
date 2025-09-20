using Microsoft.AspNetCore.Mvc;
using Caso1_Tarea.DTOs;
using Caso1_Tarea.Models;
using Caso1_Tarea.Repositories;
using Caso1_Tarea.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

[ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProveedoresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todos los proveedores
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProveedorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
        {
            var proveedores = await _unitOfWork.Proveedores.GetAllAsync();
            var proveedoresDto = proveedores.Select(p => new ProveedorDto
            {
                Proveedorid = p.Proveedorid,
                Nombre = p.Nombre,
                Contacto = p.Contacto,
                Telefono = p.Telefono
            });

            return Ok(proveedoresDto);
        }

        /// <summary>
        /// Obtiene un proveedor por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProveedorDto>> GetProveedor(int id)
        {
            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
            
            if (proveedor == null)
            {
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            var proveedorDto = new ProveedorDto
            {
                Proveedorid = proveedor.Proveedorid,
                Nombre = proveedor.Nombre,
                Contacto = proveedor.Contacto,
                Telefono = proveedor.Telefono
            };

            return Ok(proveedorDto);
        }

        /// <summary>
        /// Crea un nuevo proveedor
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProveedorDto>> CreateProveedor(CreateProveedorDto createDto)
        {
            var proveedor = new Proveedore
            {
                Nombre = createDto.Nombre,
                Contacto = createDto.Contacto,
                Telefono = createDto.Telefono
            };

            await _unitOfWork.Proveedores.AddAsync(proveedor);
            await _unitOfWork.SaveChangesAsync();

            var proveedorDto = new ProveedorDto
            {
                Proveedorid = proveedor.Proveedorid,
                Nombre = proveedor.Nombre,
                Contacto = proveedor.Contacto,
                Telefono = proveedor.Telefono
            };

            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Proveedorid }, proveedorDto);
        }

        /// <summary>
        /// Actualiza un proveedor existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProveedor(int id, UpdateProveedorDto updateDto)
        {
            if (id != updateDto.Proveedorid)
            {
                return BadRequest("El ID no coincide");
            }

            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
            if (proveedor == null)
            {
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            proveedor.Nombre = updateDto.Nombre;
            proveedor.Contacto = updateDto.Contacto;
            proveedor.Telefono = updateDto.Telefono;

            _unitOfWork.Proveedores.Update(proveedor);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina un proveedor
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
            if (proveedor == null)
            {
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            _unitOfWork.Proveedores.Remove(proveedor);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
