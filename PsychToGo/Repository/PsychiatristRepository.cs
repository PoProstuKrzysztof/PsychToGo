using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Repository;

public class PsychiatristRepository : IPsychiatristRepository
{
    private readonly AppDbContext _context;

    public PsychiatristRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ICollection<Psychiatrist>> GetPsychiatrists()
    {
        try
        {
            return await _context.Psychiatrists.OrderBy(x => x.Id).ToListAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }
}
