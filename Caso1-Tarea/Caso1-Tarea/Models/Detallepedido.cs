using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Caso1_Tarea.Models;

public partial class Detallepedido
{
    public int Detalleid { get; set; }

    public int? Pedidoid { get; set; }

    public int? Productoid { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
    public int Cantidad { get; set; }

    public virtual Pedidosproveedore? Pedido { get; set; }

    public virtual Producto? Producto { get; set; }
}