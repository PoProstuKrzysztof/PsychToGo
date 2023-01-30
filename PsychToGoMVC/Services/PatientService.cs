using PsychToGo.Models;
using PsychToGoMVC.Models;
using PsychToGoMVC.Services.Interfaces;

namespace PsychToGoMVC.Services;

public class PatientService : IPatientService
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Patient" );
    HttpClient client = new HttpClient();

    public PatientService()
    {
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

    public async Task<PatientViewModel> CreateParsedPatientInstance(int id)
    {

        Patient? findPatient = await client.GetFromJsonAsync<Patient>( client.BaseAddress + $"/{id}" );

        List<Medicine>? medicine = await client.GetFromJsonAsync<List<Medicine>>( client.BaseAddress + $"/{id}/medicines" );

        int psychiatristId = await client.GetFromJsonAsync<int>( client.BaseAddress + $"/{id}/psychiatrist" );

        int psychologistId = await client.GetFromJsonAsync<int>( client.BaseAddress + $"/{id}/psychologist" );

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
            MedicineId = medicine.First().Id
        };

        return parsedPatient;
    }

    public Patient CreatePatientInstance(PatientViewModel pvm)
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
            PsychologistId = pvm.PsychiatristId,
            PsychiatristId = pvm.PsychologistId
        };

        return newPatient;
    }
}
