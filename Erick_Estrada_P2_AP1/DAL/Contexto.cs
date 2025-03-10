using Erick_Estrada_P2_AP1.Models;
using Microsoft.EntityFrameworkCore;

namespace Erick_Estrada_P2_AP1.DAL;

public class Contexto : DbContext
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }
    public DbSet<Ciudades> Ciudades { get; set; }
    public DbSet<Encuestas> Encuestas { get; set; }
    public DbSet<EncuestasDetalle> EncuestasDetalle { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ciudades>().HasData(
            new List<Ciudades>()
            {
                new()
                {
                    CiudadId = 1,
                    Nombre = "Cotui",
                    Monto = 0
                },
                new()
                {
                   CiudadId = 2,
                   Nombre = "Azua",
                   Monto = 0
                },
                new()
                {
                 CiudadId = 3,
                 Nombre = "San Francisco de Macoris",
                 Monto = 0
                }
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}