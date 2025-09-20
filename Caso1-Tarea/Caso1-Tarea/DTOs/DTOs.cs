using System.ComponentModel.DataAnnotations;
using Caso1_Tarea.Models.Enums;

namespace Caso1_Tarea.DTOs
{
    // DTOs para Producto
    public class ProductoDto
    {
        public int Productoid { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
        public int Cantidadinventario { get; set; }
        public int? Stockminimo { get; set; }
    }

    public class CreateProductoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(255)]
        public string Nombre { get; set; } = null!;
        
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Precio { get; set; }
        
        [StringLength(100)]
        public string? Categoria { get; set; }
        
        [Required(ErrorMessage = "La cantidad en inventario es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser 0 o mayor")]
        public int Cantidadinventario { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser 0 o mayor")]
        public int? Stockminimo { get; set; }
    }

    public class UpdateProductoDto : CreateProductoDto
    {
        public int Productoid { get; set; }
    }

    // DTOs para Proveedor
    public class ProveedorDto
    {
        public int Proveedorid { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
    }

    public class CreateProveedorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(255)]
        public string Nombre { get; set; } = null!;
        
        [StringLength(255)]
        public string? Contacto { get; set; }
        
        [StringLength(20)]
        [Phone(ErrorMessage = "El teléfono no tiene un formato válido")]
        public string? Telefono { get; set; }
    }

    public class UpdateProveedorDto : CreateProveedorDto
    {
        public int Proveedorid { get; set; }
    }

    // DTOs para PedidosProveedores
    public class PedidoProveedorDto
    {
        public int Pedidoid { get; set; }
        public int? Proveedorid { get; set; }
        public DateOnly? Fechapedido { get; set; }
        public EstadoPedido Estado { get; set; }
        public string? NombreProveedor { get; set; }
        public List<DetallePedidoDto> Detalles { get; set; } = new();
    }

    public class CreatePedidoProveedorDto
    {
        [Required(ErrorMessage = "El proveedor es requerido")]
        public int Proveedorid { get; set; }
        
        [Required(ErrorMessage = "La fecha del pedido es requerida")]
        public DateOnly Fechapedido { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        public EstadoPedido Estado { get; set; }
        
        public List<CreateDetallePedidoDto> Detalles { get; set; } = new();
    }

    public class UpdatePedidoProveedorDto
    {
        public int Pedidoid { get; set; }
        public EstadoPedido Estado { get; set; }
    }

    // DTOs para DetallePedido
    public class DetallePedidoDto
    {
        public int Detalleid { get; set; }
        public int? Pedidoid { get; set; }
        public int? Productoid { get; set; }
        public int Cantidad { get; set; }
        public string? NombreProducto { get; set; }
        public decimal? PrecioUnitario { get; set; }
    }

    public class CreateDetallePedidoDto
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int Productoid { get; set; }
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
    }

    // DTOs para MovimientosInventario
    public class MovimientoInventarioDto
    {
        public int Movimientoid { get; set; }
        public int? Productoid { get; set; }
        public TipoMovimiento Tipomovimiento { get; set; }
        public int Cantidad { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NombreProducto { get; set; }
    }

    public class CreateMovimientoInventarioDto
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int Productoid { get; set; }
        
        [Required(ErrorMessage = "El tipo de movimiento es requerido")]
        public TipoMovimiento Tipomovimiento { get; set; }
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
    }
}