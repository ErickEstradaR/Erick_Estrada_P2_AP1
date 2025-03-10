using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erick_Estrada_P2_AP1.Models;

public class EncuestasDetalle
{
    [Key]
    public int DetalleId { get; set; }
    
    [ForeignKey("Encuestas")]
    public int EncuestaId { get; set; }
    
    public Encuestas? Encuesta { get; set; }
    
    [Required (ErrorMessage = "Ingresa un valor valido")]
    [Range(1, double.MaxValue)]
    public int Monto { get; set; }
    
    public int cantidadEncuestas { get; set; }
}