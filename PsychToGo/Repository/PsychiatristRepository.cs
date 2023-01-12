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
    //Post

    public async Task<bool> Save()
    {
        try
        {
            var savedEntity = await _context.SaveChangesAsync();
            return savedEntity > 0 ? true : false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckDuplicate(PsychiatristDTO psychiatrist)
    {
        try
        {
            var psychiatrists = await _context.Psychiatrists.ToListAsync();
            var psychiatristDuplicate = psychiatrists
                .Where( x => x.Email == psychiatrist.Email )
                .FirstOrDefault();

            if (psychiatristDuplicate != null)
            {
                return true;
            }

            return false;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CreatePsychiatrist(Psychiatrist psychiatrist)
    {
        try
        {
            await _context.AddAsync( psychiatrist );
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }
    //Get
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

    public async Task<bool> UpdatePsychiatrist(Psychiatrist psychiatrist)
    {
        try
        {
            _context.Update( psychiatrist );
            return await Save();
        }
        catch(Exception )
        {
            throw;
        }
    }
}
