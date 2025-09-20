using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Caso1_Tarea.Models.Enums;

namespace Caso1_Tarea.Models;

public partial class Pedidosproveedore
{
    public int Pedidoid { get; set; }

    public int? Proveedorid { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Fechapedido { get; set; }

    [Required]
    public EstadoPedido Estado { get; set; }

    public virtual ICollection<Detallepedido> Detallepedidos { get; set; } = new List<Detallepedido>();

    public virtual Proveedore? Proveedor { get; set; }
}