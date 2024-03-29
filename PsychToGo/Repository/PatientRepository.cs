﻿using Microsoft.EntityFrameworkCore;
using PsychToGo.API.Data;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGo.API.Repository;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    //Put
    public async Task<bool> UpdatePatient(Patient patient)
    {
        try
        {
            _context.Update(patient);

            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> AssignMedicine(int patientId, int medicineId)
    {
        try
        {
            Patient? patientEntity = await _context.Patients.
                FirstOrDefaultAsync(x => x.Id == patientId);

            Medicine? medicineEntity = await _context.Medicines
                .Where(x => x.Id == medicineId)
                .FirstOrDefaultAsync();

            PatientMedicine relationship = new PatientMedicine()
            {
                Medicine = medicineEntity,
                Patient = patientEntity
            };

            await _context.AddAsync(relationship);

            return await Save();
        }
        catch (DbUpdateException)
        {
            return false;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> AssignPsychiatrist(int patientId, int psychiatristId)
    {
        try
        {
            Patient? patient = await _context.Patients
                .FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                return false;
            }

            patient.Psychiatrist = await _context.Psychiatrists
                .FirstOrDefaultAsync(x => x.Id == psychiatristId);
            return await Save();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    //Post
    public async Task<bool> CreatePatient(int medicineId, Patient patient)
    {
        try
        {
            Medicine? medicineEntity = await _context.Medicines
                .Where(x => x.Id == medicineId)
                .FirstOrDefaultAsync();

            PatientMedicine relationship = new PatientMedicine()
            {
                Medicine = medicineEntity,
                Patient = patient
            };
            await _context.AddAsync(relationship);

            await _context.AddAsync(patient);
            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> CreatePatientWithoutPsychiatrist(Patient patient)
    {
        try
        {
            await _context.AddAsync(patient);
            return await Save();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

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

    //Get
    public async Task<Patient> GetPatientById(int id)
    {
        try
        {
            Patient? patient = await _context.Patients.Where(p => p.Id == id).FirstOrDefaultAsync();

            return patient;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<Patient> GetPatientByName(string name)
    {
        try
        {
            Patient? patient = await _context.Patients
                .Where(p => p.Name == name).FirstOrDefaultAsync();

            return patient;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<ICollection<Medicine>?> GetPatientMedicines(int id)
    {
        try
        {
            Patient? patient = await _context.Patients
                .Where(x => x.Id == id)
                .AsNoTracking().
                FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            List<Medicine> patientMedicines = await _context.PatientMedicines
                .Where(x => x.PatientId == id)
                .AsNoTracking()
                .Select(x => x.Medicine)
                .ToListAsync();

            if (patientMedicines == null)
            {
                return null;
            }

            return patientMedicines;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<ICollection<Patient>> GetPatients()
    {
        try
        {
            return await _context.Patients.
                OrderBy(x => x.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> PatientExists(int id)
    {
        try
        {
            return await _context.Patients
                .AnyAsync(x => x.Id == id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    //Delete
    public async Task<bool> DeletePatient(Patient patient)
    {
        try
        {
            _context.Remove(patient);
            return await Save();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<Psychologist?> GetPatientPsychologist(int id)
    {
        try
        {
            Patient patient = await GetPatientById(id);
            if (!await PatientExists(patient.Id))
            {
                return null;
            }

            Psychologist? patientPsychologist = await _context.Psychologists
                .Where(x => x.Id == patient.PsychologistId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return patientPsychologist;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<Psychiatrist?> GetPatientPsychiatrist(int id)
    {
        try
        {
            Patient patient = await GetPatientById(id);
            if (!await PatientExists(patient.Id))
            {
                return null;
            }
            Psychiatrist? patientPsychiatrist = await _context.Psychiatrists
                .Where(x => x.Id == patient.PsychiatristId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return patientPsychiatrist;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<bool> CheckDuplicate(PatientDTO patient)
    {
        try
        {
            List<Patient> patients = await _context.Patients.AsNoTracking().ToListAsync();
            Patient? patientDuplicate = patients.
                Where(x => x.Email == patient.Email).FirstOrDefault();
            if (patientDuplicate != null)
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
}