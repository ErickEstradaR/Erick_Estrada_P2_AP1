using System.ComponentModel.DataAnnotations;
namespace Erick_Estrada_P2_AP1.Models;

public class Ciudades
{
    [Key]
    public int CiudadId { get; set; }
    
    [Required]
    public string Nombre { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public double Monto { get; set; }
}