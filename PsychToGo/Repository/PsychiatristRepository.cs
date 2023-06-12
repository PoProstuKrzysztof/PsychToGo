using Microsoft.EntityFrameworkCore;
using PsychToGo.API.Data;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGo.API.Repository;

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
            int savedEntity = await _context.SaveChangesAsync();
            return savedEntity > 0 ? true : false;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> CheckDuplicate(PsychiatristDTO psychiatrist)
    {
        try
        {
            List<Psychiatrist> psychiatrists = await _context.Psychiatrists
                .AsNoTracking()
                .ToListAsync();
            Psychiatrist? psychiatristDuplicate = psychiatrists
                .Where(x => x.Email == psychiatrist.Email)
                .FirstOrDefault();

            if (psychiatristDuplicate != null)
            {
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> CreatePsychiatrist(Psychiatrist psychiatrist)
    {
        try
        {
            await _context.AddAsync(psychiatrist);
            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    //Get
    public async Task<Psychiatrist> GetPsychiatrist(int id)
    {
        try
        {
            Psychiatrist? findPsychiatrist = await _context.Psychiatrists
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (findPsychiatrist == null)
            {
                return null;
            }

            return findPsychiatrist;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<ICollection<Patient>> GetPsychiatristPatients(int id)
    {
        try
        {
            List<Patient> psychiatristPatients = await _context.Patients
                .Where(x => x.PsychiatristId == id)
                .Select(p => p)
                .ToListAsync();
            if (psychiatristPatients == null)
            {
                return null;
            }

            return psychiatristPatients;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<ICollection<Psychiatrist>> GetPsychiatrists()
    {
        try
        {
            return await _context.Psychiatrists
                .OrderBy(x => x.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> PsychiatristExist(int id)
    {
        try
        {
            return await _context.Psychiatrists.AnyAsync(x => x.Id == id);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> UpdatePsychiatrist(Psychiatrist psychiatrist)
    {
        try
        {
            _context.Update(psychiatrist);
            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    //Delete
    public async Task<bool> DeletePsychiatrist(Psychiatrist psychiatrist)
    {
        try
        {
            _context.Remove(psychiatrist);
            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }
}