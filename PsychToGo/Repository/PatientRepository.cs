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
}
