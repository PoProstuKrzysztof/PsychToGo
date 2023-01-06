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

    public async Task<Psychologist> GetPsychologist(int id)
    {
        try
        {
            
            var findPsychologist = await _context.Psychologists.Where( x => x.Id == id ).FirstOrDefaultAsync();
            if(findPsychologist == null )
            {
                return null;
            }


            return findPsychologist;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public Task<Psychologist> GetPsychologist(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Patient>> GetPsychologistPatients(int id)
    {
        try
        {
            var findPsychologist = await _context.Psychologists.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(findPsychologist == null)
            {
                return null;
            }


            var psychologistPatients =  findPsychologist.Patients.ToList();
            if(psychologistPatients == null)
            {
                return null;
            }

            return psychologistPatients;

        }
        catch(Exception ex)
        {
            throw;
        }
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

    public async Task<bool> PsychologistExist(int id)
    {
        try
        {
            return await _context.Psychologists.AnyAsync( x => x.Id == id );

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
