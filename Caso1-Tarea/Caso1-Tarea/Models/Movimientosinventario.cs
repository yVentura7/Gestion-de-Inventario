using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Caso1_Tarea.Models.Enums;

namespace Caso1_Tarea.Models;

public partial class Movimientosinventario
{
    public int Movimientoid { get; set; }

    public int? Productoid { get; set; }

    [Required]
    public TipoMovimiento Tipomovimiento { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
    public int Cantidad { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Producto? Producto { get; set; }
}