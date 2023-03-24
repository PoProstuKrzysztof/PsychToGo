using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGoMVC.Models;
using PsychToGoMVC.Services.Interfaces;

namespace PsychToGoMVC.Services;

public class PatientService : IPatientService
{
    Uri baseAddress = new Uri( "https://localhost:7291/api/Patient" );
    Uri psychiatristAddress = new Uri( "https://localhost:7291/api/Psychiatrist" );
    Uri psychologistAddress = new Uri( "https://localhost:7291/api/Psychologist" );
    Uri medicinesAddress = new Uri( "https://localhost:7291/api/Medicine" );
    HttpClient client = new HttpClient();

    public PatientService()
    {
        client = new HttpClient();
        client.BaseAddress = baseAddress;
        
    }

    public async Task<PatientViewModel> CreateParsedPatientInstance(int id)
    {

        Patient? findPatient = await client.GetFromJsonAsync<Patient>( client.BaseAddress + $"/{id}" );


        int psychologistId = await client.GetFromJsonAsync<int>( client.BaseAddress + $"/{id}/psychologist" );
        var psychiatristId =  await client.GetFromJsonAsync<int>( client.BaseAddress + $"/{id}/psychiatrist" );
        


        if (psychiatristId == 0)
        {
            PatientViewModel parsedPatientNoPsychiatrist = new PatientViewModel()
            {
                Name = findPatient.Name,
                LastName = findPatient.LastName,
                Email = findPatient.Email,
                Address = findPatient.Address,
                DateOfBirth = findPatient.DateOfBirth,
                Phone = findPatient.Phone,
                PsychologistId = psychologistId,
                Id = id
            };

            return parsedPatientNoPsychiatrist;
        }


        List<Medicine>? medicines = await client.GetFromJsonAsync<List<Medicine>>( client.BaseAddress + $"/{id}/medicines" );
       

        

        PatientViewModel parsedPatient = new PatientViewModel()
        {
            Name = findPatient.Name,
            LastName = findPatient.LastName,
            Email = findPatient.Email,
            Address = findPatient.Address,
            DateOfBirth = findPatient.DateOfBirth,
            Phone = findPatient.Phone,
            PsychiatristId = psychiatristId,
            PsychologistId = psychologistId,
            MedicinesId = medicines.Select( m => m.Id ).ToList(),
            Id = id


        };

        return parsedPatient;
    }

    public async Task<Patient> CreatePatientInstance(PatientViewModel pvm)
    {

        Patient newPatient = new Patient()
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
        var psychiatrists = await client.GetFromJsonAsync<ICollection<PsychiatristDTO>>( psychiatristAddress + $"/list" );
        return psychiatrists;
    }

    public async Task<ICollection<PsychologistDTO>> PsychologistsList()
    {
        var psychologists = await client.GetFromJsonAsync<ICollection<PsychologistDTO>>( psychologistAddress + $"/list" );
        return psychologists;
    }

    public async Task<ICollection<MedicineDTO>> MedicinesList()
    {
        List<MedicineDTO>? medicines = await client.GetFromJsonAsync<List<MedicineDTO>>( medicinesAddress + $"/list" );
        
        return medicines;

    }
}
