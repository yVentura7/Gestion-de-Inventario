using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Caso1_Tarea.Models;

public partial class Producto
{
    public int Productoid { get; set; }

    [Required]
    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    public decimal Precio { get; set; }

    [StringLength(100)]
    public string? Categoria { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser 0 o mayor")]
    public int Cantidadinventario { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser 0 o mayor")]
    public int? Stockminimo { get; set; }

    public virtual ICollection<Detallepedido> Detallepedidos { get; set; } = new List<Detallepedido>();

    public virtual ICollection<Movimientosinventario> Movimientosinventarios { get; set; } = new List<Movimientosinventario>();
}