using System.ComponentModel.DataAnnotations;
namespace Erick_Estrada_P2_AP1.Models;

public class Encuestas
{
    [Key]
    public int EncuestaId { get; set; }
    public DateTime FechaEncuesta { get; set; }
    [Required]
    [MaxLength(50)]
    public string Asignatura { get; set; }
    public List<EncuestasDetalle> EncuestasDetalles { get; set; } = new List<EncuestasDetalle>();
    public double MontoTotal { get; set; }
}