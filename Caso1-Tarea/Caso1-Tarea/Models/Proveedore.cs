using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Caso1_Tarea.Models;

public partial class Proveedore
{
    public int Proveedorid { get; set; }

    [Required]
    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Contacto { get; set; }

    [StringLength(20)]
    [Phone]
    public string? Telefono { get; set; }

    public virtual ICollection<Pedidosproveedore> Pedidosproveedores { get; set; } = new List<Pedidosproveedore>();
}