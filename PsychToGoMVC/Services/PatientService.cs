using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PatientService : IPatientService
{
    private readonly Uri baseAddress = new( "https://localhost:7291/api/Patient" );
    private readonly Uri psychiatristAddress = new( "https://localhost:7291/api/Psychiatrist" );
    private readonly Uri psychologistAddress = new( "https://localhost:7291/api/Psychologist" );
    private readonly Uri medicinesAddress = new( "https://localhost:7291/api/Medicine" );
    private readonly HttpClient client = new();

    public PatientService()
    {
        client.BaseAddress = baseAddress;
    }

    public async Task<PatientViewModel> CreateParsedPatientInstance(int id)
    {
        Patient? findPatient = await client.GetFromJsonAsync<Patient>( client.BaseAddress + $"/{id}" );

        PsychologistDTO psychologist = await client.GetFromJsonAsync<PsychologistDTO>( client.BaseAddress + $"/{id}/psychologist" );
        PsychiatristDTO psychiatrist = await client.GetFromJsonAsync<PsychiatristDTO>( client.BaseAddress + $"/{id}/psychiatrist" );

        if (psychiatrist == null)
        {
            PatientViewModel parsedPatientNoPsychiatrist = new()
            {
                Name = findPatient.Name,
                LastName = findPatient.LastName,
                Email = findPatient.Email,
                Address = findPatient.Address,
                DateOfBirth = findPatient.DateOfBirth,
                Phone = findPatient.Phone,
                PsychologistId = psychologist.Id,
                Id = id
            };

            return parsedPatientNoPsychiatrist;
        }

        List<Medicine>? medicines = await client.GetFromJsonAsync<List<Medicine>>( client.BaseAddress + $"/{id}/medicines" );

        PatientViewModel parsedPatient = new()
        {
            Name = findPatient.Name,
            LastName = findPatient.LastName,
            Email = findPatient.Email,
            Address = findPatient.Address,
            DateOfBirth = findPatient.DateOfBirth,
            Phone = findPatient.Phone,
            PsychiatristId = psychiatrist.Id,
            PsychologistId = psychologist.Id,
            MedicinesId = medicines.Select( m => m.Id ).ToList(),
            Id = id
        };

        return parsedPatient;
    }

    public async Task<Patient> CreatePatientInstance(PatientViewModel pvm)
    {
        Patient newPatient = new()
        {
            Id = pvm.Id,
            Name = pvm.Name,
            LastName = pvm.LastName,
            Email = pvm.Email,
            Address = pvm.Address,
            DateOfBirth = pvm.DateOfBirth,
            Phone = pvm.Phone,
            PsychologistId = pvm.PsychologistId,
            PsychiatristId = pvm.PsychiatristId
        };

        return newPatient;
    }

    public async Task<ICollection<PsychiatristDTO>> PsychiatristsList()
    {
        ICollection<PsychiatristDTO>? psychiatrists = await client.GetFromJsonAsync<ICollection<PsychiatristDTO>>( psychiatristAddress + $"/list" );
        return psychiatrists;
    }

    public async Task<ICollection<PsychologistDTO>> PsychologistsList()
    {
        ICollection<PsychologistDTO>? psychologists = await client.GetFromJsonAsync<ICollection<PsychologistDTO>>( psychologistAddress + $"/list" );
        return psychologists;
    }

    public async Task<ICollection<MedicineDTO>> MedicinesList()
    {
        List<MedicineDTO>? medicines = await client.GetFromJsonAsync<List<MedicineDTO>>( medicinesAddress + $"/list" );

        return medicines;
    }
}