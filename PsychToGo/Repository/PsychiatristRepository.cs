using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.DTO;
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

    public Task<bool> CheckDuplicate(PsychiatristDTO psychiatrist)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreatePsychiatrist(Psychiatrist psychiatrist)
    {
        throw new NotImplementedException();
    }

    public async Task<Psychiatrist> GetPsychiatrist(int id)
    {
        try
        {
            var findPsychiatrist = await _context.Psychiatrists.Where( x => x.Id == id ).FirstOrDefaultAsync();
            if (findPsychiatrist == null)
            {
                return null;
            }


            return findPsychiatrist;
        }
        catch(Exception) 
        {
            throw;
        }
    }

    public async Task<ICollection<Patient>> GetPsychiatristPatients(int id)
    {
        try
        {
            var psychiatristPatients = await _context.Patients
                .Where( x => x.PsychiatristId == id )
                .Select( p => p )
                .ToListAsync();
            if(psychiatristPatients == null)
            {
                return null;
            }

            return psychiatristPatients;
        }
        catch(Exception)
        {
            throw;
        }
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

    public async Task<bool> PsychiatristExist(int id)
    {
        try
        {
            return await _context.Psychiatrists.AnyAsync( x => x.Id == id );
        }
        catch(Exception)
        {
            throw;
        }
        
    }

    public Task<bool> Save()
    {
        throw new NotImplementedException();
    }
}
