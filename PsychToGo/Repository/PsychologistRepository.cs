using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Repository;

public class PsychologistsRepository : IPsychologistRepository
{
    private readonly AppDbContext _context;
    public PsychologistsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Psychologist>> GetPsychologists()
    {
        try
        {
            return await _context.Psychologists.OrderBy(x => x.Id).ToListAsync();
        }
        catch(Exception e)
        {
            throw;
        }
        
    }
}
