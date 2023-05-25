using Microsoft.EntityFrameworkCore;
using PsychToGo.API.Data;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGo.API.Repository;

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
            int savedEntity = await _context.SaveChangesAsync();
            return savedEntity > 0 ? true : false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckDuplicate(PsychologistDTO psychologist)
    {
        try
        {
            List<Psychologist> psychologists = await _context.Psychologists.ToListAsync();
            Psychologist? psychologistDuplicate = psychologists
                .Where(x => x.Email == psychologist.Email)
                .FirstOrDefault();

            if (psychologistDuplicate != null)
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

    public async Task<bool> CreatePsychologist(Psychologist psychologist)
    {
        try
        {
            await _context.AddAsync(psychologist);
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }

    //Get
    public async Task<Psychologist> GetPsychologist(int id)
    {
        try
        {
            Psychologist? findPsychologist = await _context.Psychologists.Where(x => x.Id == id).FirstOrDefaultAsync();

            return findPsychologist;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<Patient>> GetPsychologistPatients(int id)
    {
        try
        {
            List<Patient> psychologistPatients = await _context.Patients
                .Where(x => x.PsychologistId == id)
                .Select(p => p)
                .ToListAsync();
            if (psychologistPatients == null)
            {
                return new List<Patient>();
            }

            return psychologistPatients;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<Psychologist>> GetPsychologists()
    {
        try
        {
            return await _context.Psychologists
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> PsychologistExist(int id)
    {
        try
        {
            return await _context.Psychologists.AnyAsync(x => x.Id == id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    //Put
    public async Task<bool> UpdatePsychologist(Psychologist psychologist)
    {
        try
        {
            _context.Update(psychologist);
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }

    //Delete
    public async Task<bool> DeletePsychologist(Psychologist psychologist)
    {
        try
        {
            _context.Remove(psychologist);
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }
}