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
    
    private async Task AfectarMontoCiudad(IEnumerable<EncuestasDetalle> detalles, bool suma)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        foreach (var detalle in detalles)
        {
            var ciudad = await contexto.Ciudades.FindAsync(detalle.CiudadId);
            if (ciudad != null)
            {
                ciudad.Monto += suma ? detalle.Monto : -detalle.Monto;
            }
        }
        await contexto.SaveChangesAsync();
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
        await AfectarMontoCiudad(encuestas.EncuestasDetalles, true);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        var encuesta = await contexto.Encuestas.Include(e => e.EncuestasDetalles).FirstOrDefaultAsync(e => e.EncuestaId == id);

        if (encuesta == null)
            return false;

        await AfectarMontoCiudad(encuesta.EncuestasDetalles, false);
        contexto.Encuestas.Remove(encuesta);
        return await contexto.SaveChangesAsync() > 0;
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
        await using var contexto = await dbfactory.CreateDbContextAsync();

        var encuestaOriginal = await contexto.Encuestas.Include(e => e.EncuestasDetalles).FirstOrDefaultAsync(e => e.EncuestaId == encuestas.EncuestaId);

        if (encuestaOriginal == null)
            return false;

        await AfectarMontoCiudad(encuestaOriginal.EncuestasDetalles, false);
        contexto.Entry(encuestaOriginal).CurrentValues.SetValues(encuestas);

        foreach (var detalle in encuestas.EncuestasDetalles)
        {
            var detalleExistente = encuestaOriginal.EncuestasDetalles.FirstOrDefault(d => d.DetalleId == detalle.DetalleId);
            if (detalleExistente != null)
            {
                contexto.Entry(detalleExistente).CurrentValues.SetValues(detalle);
            }
            else
            {
                encuestaOriginal.EncuestasDetalles.Add(detalle);
            }
        }

        await AfectarMontoCiudad(encuestas.EncuestasDetalles, true);
        return await contexto.SaveChangesAsync() > 0;
    }
    
    public async Task<Encuestas?> Buscar(int id)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        return await contexto.Encuestas
            .Where(e => e.EncuestaId == id)
            .Include(e => e.EncuestasDetalles)
            .ThenInclude(ed => ed.Ciudad) 
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Encuestas>> Listar(Expression<Func<Encuestas, bool>> criterio)
    {
        await using var contexto = await dbfactory.CreateDbContextAsync();
        return await contexto.Encuestas
            .Include(e => e.EncuestasDetalles)
            .ThenInclude(ed => ed.Ciudad) 
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

}