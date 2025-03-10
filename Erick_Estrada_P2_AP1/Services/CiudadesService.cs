using System.Linq.Expressions;
using Erick_Estrada_P2_AP1.DAL;
using Erick_Estrada_P2_AP1.Models;
using Microsoft.EntityFrameworkCore;

namespace Erick_Estrada_P2_AP1.Services;

public class CiudadesService (IDbContextFactory<Contexto> dbContextFactory)
{
    
    public async Task<Ciudades> Buscar(int id)
    {
        await using var contexto = await dbContextFactory.CreateDbContextAsync();
        return await contexto.Ciudades.Where(c=>c.CiudadId==id).FirstOrDefaultAsync();
    }

    public async Task<List<Ciudades>> Listar(Expression<Func<Ciudades, bool>> criterio)
    {
        await using var contexto = await dbContextFactory.CreateDbContextAsync();
        return  await contexto.Ciudades.AsNoTracking().Where(criterio).ToListAsync();
    }
    
}