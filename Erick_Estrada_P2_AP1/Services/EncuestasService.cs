using System.Linq.Expressions;
using Erick_Estrada_P2_AP1.DAL;
using Erick_Estrada_P2_AP1.Models;
using Microsoft.EntityFrameworkCore;

namespace Erick_Estrada_P2_AP1.Services;

public class EncuestasService (IDbContextFactory<Contexto> dbfactory)
{
    public async Task<bool> Guardar(Encuestas encuesta)
    {
        if (!await Existe(encuesta.EncuestaId))
        {
            return await Insertar(encuesta);
        }
        return await Modificar(encuesta);
    }
    
    public async Task<bool> Existe(int Id)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
       return await contexto.Encuestas.AnyAsync(e => e.EncuestaId == Id);
    }
    
    public async Task<bool> Insertar(Encuestas encuestas)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        contexto.Encuestas.Add(encuestas);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await dbfactory
            .CreateDbContextAsync();
        var encuestas = await contexto.Encuestas
            .Include(e=>e.EncuestasDetalles)
            .FirstOrDefaultAsync(e => e.EncuestaId == id);

        if (encuestas == null)
            return false;


        await AfectarEncuesta(encuestas.EncuestasDetalles.ToArray(), suma: false);
        contexto.EncuestasDetalle.RemoveRange(encuestas.EncuestasDetalles);

        var cantidad = await contexto.SaveChangesAsync();
        return cantidad > 0;
    }

    private async Task AfectarEncuesta(EncuestasDetalle[] detalle, bool suma = true)
    {
        await using var contexto = await dbfactory
            .CreateDbContextAsync();
        foreach (var item in detalle)
        {
            var encuesta = await contexto.Encuestas.SingleAsync(e => e.EncuestaId == item.EncuestaId);
         
            await contexto.SaveChangesAsync();
        }
    }
    
    public async Task<bool> Modificar(Encuestas encuestas)
    {
        await using var contexto = await dbfactory
            .CreateDbContextAsync();

        var encuestaOriginal = await contexto.Encuestas
            .Include(e=>e.EncuestasDetalles)
            .FirstOrDefaultAsync(e =>e.EncuestaId == encuestas.EncuestaId);

        if (encuestaOriginal == null)
            return false;

        await AfectarEncuesta(encuestaOriginal.EncuestasDetalles.ToArray(), false);

        foreach (var detalleOriginal in encuestaOriginal.EncuestasDetalles)
        {
            if (!encuestas.EncuestasDetalles.Any(d => d.DetalleId == detalleOriginal.DetalleId))
            {
                contexto.EncuestasDetalle.Remove(detalleOriginal);
            }
        }

        await AfectarEncuesta(encuestaOriginal.EncuestasDetalles.ToArray(), true);
        contexto.Entry(encuestaOriginal).CurrentValues.SetValues(encuestas);

        foreach (var detalle in encuestas.EncuestasDetalles)
        {
            var detalleExistente = encuestas.EncuestasDetalles
                .FirstOrDefault(d => d.DetalleId == detalle.DetalleId);

            if (detalleExistente != null)
            {
                contexto.Entry(detalleExistente).CurrentValues.SetValues(detalle);
            }
            else
            {
                encuestaOriginal.EncuestasDetalles.Add(detalle);
            }
        }

        return await contexto.SaveChangesAsync() > 0;
    }
    
    public async Task<Encuestas?> Buscar(int id)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        return await contexto.Encuestas.Where(e =>e.EncuestaId == id).Include(e=>e.EncuestasDetalles).
            AsNoTracking()
            .FirstOrDefaultAsync();
    }
    

    public async Task<List<Encuestas>> Listar(Expression<Func<Encuestas, bool>>criterio)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        return await contexto.Encuestas.
            Include(e=>e.EncuestasDetalles).
            Where
            (criterio).AsNoTracking().
            ToListAsync();
    }
}