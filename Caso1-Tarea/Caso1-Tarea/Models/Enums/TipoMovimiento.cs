using System.ComponentModel.DataAnnotations;

namespace Caso1_Tarea.Models.Enums
{
    public enum TipoMovimiento
    {
        [Display(Name = "Entrada")]
        Entrada,
        
        [Display(Name = "Salida")]
        Salida
    }
}