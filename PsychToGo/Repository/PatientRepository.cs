﻿using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.DTO;
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

    //Post 

    public async Task<bool> UpdatePatient(int psychologistId, int psychiatristId, int medicineId, Patient patient)
    {
        try
        {
            _context.Update(patient);

            return await Save();

        }
        catch(Exception)
        {
            throw;
        }
    }


    public async Task<bool> CreatePatient(int medicineId, Patient patient)
    {
        try
        {
            var medicineEntity = await _context.Medicines.Where( x => x.Id == medicineId ).FirstOrDefaultAsync();

            var patientMedicine = new PatientMedicine()
            {
                Medicine = medicineEntity,
                Patient = patient
            };
            await _context.AddAsync( patientMedicine );

            await _context.AddAsync( patient );
            return await Save();

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckDuplicate(PatientDTO patient)
    {
        try
        {
            var patients = await _context.Patients.ToListAsync();
            var patientDuplicate = patients.Where( x => x.Email == patient.Email ).FirstOrDefault();
            if (patientDuplicate != null)
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

    //Get 
    public async Task<Patient> GetPatientById(int id)
    {
        try
        {

            var patient = await _context.Patients.Where( p => p.Id == id ).FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            return patient;
        }
        catch (Exception)
        {
            throw;
        }


    }

    public async Task<Patient> GetPatientByName(string name)
    {
        try
        {
            var patient = await _context.Patients.Where( p => p.Name == name ).FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            return patient;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<Medicine>?> GetPatientMedicines(int id)
    {
        try
        {
            var patient = await _context.Patients.Where( x => x.Id == id ).FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            var patientMedicines = await _context.PatientMedicines
                .Where( x => x.PatientId == id )
                .Select( x => x.Medicine )
                .ToListAsync();
            if (patientMedicines == null)
            {
                return null;
            }


            return patientMedicines;

        }
        catch (Exception)
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
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<bool> PatientExists(int id)
    {
        try
        {
            return await _context.Patients.AnyAsync( x => x.Id == id );
        }
        catch (Exception)
        {
            throw;
        }
    }

  
}