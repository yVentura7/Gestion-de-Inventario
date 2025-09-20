using System.ComponentModel.DataAnnotations;

namespace Caso1_Tarea.Models.Enums
{
    public enum EstadoPedido
    {
        [Display(Name = "Pendiente")] Pendiente,

        [Display(Name = "Enviado")] Enviado,

        [Display(Name = "Entregado")] Entregado
    }
}