using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Repository;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;
    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetPatient(int id)
    {
        try
        {
            
            var patientFoundById = await _context.Patients.Where( p => p.Id == id ).FirstOrDefaultAsync();
            if (patientFoundById == null)
            {
                return null;
            }

            return patientFoundById;
        }
        catch(Exception ex)
        {
            throw;
        }
        

    }

    public async Task<Patient?> GetPatient(string name)
    {
        try
        {
            var patientFoundByString = await _context.Patients.Where(p => p.Equals(name)).FirstOrDefaultAsync();
            if (patientFoundByString == null)
            {
                return null;
            }

            return patientFoundByString;

        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<ICollection<Medicine>?> GetPatientMedicines(int id)
    {
        try
        {
            var patient = await _context.Patients.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            var patientMedicines = patient.PatientMedicines.Select(x => x.Medicine).ToList();
            if(patientMedicines == null)
            {
                return null;
            }


            return patientMedicines;
            
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<ICollection<Patient>> GetPatients()
    {
        try
        {
            return await _context.Patients.OrderBy( x => x.Id ).ToListAsync();
        }
        catch(Exception ex)
        {
            throw; 
        }
        
    }

    public async Task<bool> PatientExists(int id)
    {
        try
        {
            return await _context.Patients.AnyAsync(x => x.Id == id);
        }
        catch(Exception ex)
        {
            throw; 
        }
    }
}
