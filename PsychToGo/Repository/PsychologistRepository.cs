using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.DTO;
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
    //Post

    public async Task<bool> Save()
    {
        try
        {
            var savedEntity = await _context.SaveChangesAsync();
            return savedEntity > 0 ? true : false;
        }
        catch(Exception)
        {
            throw;
        }
        
    }

    public async Task<bool> CheckDuplicate(PsychologistDTO psychologist)
    {
        try
        {
            var psychologists = await _context.Psychologists.ToListAsync();
            var psychologistDuplicate = psychologists
                .Where( x => x.Email == psychologist.Email )
                .FirstOrDefault();

            if(psychologistDuplicate != null)
            {
                return true;
            }

            return false;

        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<bool> CreatePsychologist(Psychologist psychologist)
    {
        try
        {
            await _context.AddAsync( psychologist );
            return await Save();
        }
        catch(Exception )
        {
            throw;
        }
    }
    //Get
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


    public async Task<ICollection<Patient>> GetPsychologistPatients(int id)
    {
        try
        {


            var psychologistPatients =  await _context.Patients
                .Where(x => x.PsychologistId == id)
                .Select(p => p )
                .ToListAsync();
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
    //Put
    public async Task<bool> UpdatePsychologist(Psychologist psychologist)
    {
        try
        {
            _context.Update( psychologist );
            return await Save();
        }
        catch(Exception ) 
        { 
            throw;
        }

    }
    //Delete
    public async Task<bool> DeletePsychologist(Psychologist psychologist)
    {
        try
        {
            _context.Remove( psychologist );
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
