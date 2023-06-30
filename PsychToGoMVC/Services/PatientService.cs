using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PatientService : IPatientService
{
    private readonly Uri _baseAddress = new("https://localhost:7291/api/Patient");
    private readonly Uri _psychiatristAddress = new("https://localhost:7291/api/Psychiatrist");
    private readonly Uri _psychologistAddress = new("https://localhost:7291/api/Psychologist");
    private readonly Uri _medicinesAddress = new("https://localhost:7291/api/Medicine");
    private readonly HttpClient _client = new();

    public PatientService()
    {
        _client.BaseAddress = _baseAddress;
    }

    public async Task<PatientViewModel> CreateParsedPatientViewModel(int id)
    {
        Patient? findPatient = await _client.GetFromJsonAsync<Patient>(_client.BaseAddress + $"/{id}");

        PsychologistDTO psychologist = await _client
            .GetFromJsonAsync<PsychologistDTO>(_client.BaseAddress + $"/{id}/psychologist");

        HttpResponseMessage psychiatristCheck = await _client.GetAsync(_client.BaseAddress + $"/{id}/psychiatrist");

        if (psychiatristCheck.Content.Headers.ContentLength == 0)
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

        //Only if psychiatrist exist medicines can be searched for
        List<Medicine>? medicines = await _client.GetFromJsonAsync<List<Medicine>>(_client.BaseAddress + $"/{id}/medicines");
        var psychiatrist = JsonConvert
            .DeserializeObject<PsychiatristDTO>(psychiatristCheck.Content.ReadAsStringAsync().Result);

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
            MedicinesId = medicines.Select(m => m.Id).ToList(),
            Id = id
        };

        return parsedPatient;
    }

    public async Task<Patient> CreatePatientViewModel(PatientViewModel pvm)
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

    public async Task<ICollection<PsychiatristDTO>> GetPsychiatristsList()
    {
        ICollection<PsychiatristDTO>? psychiatrists = await _client
            .GetFromJsonAsync<ICollection<PsychiatristDTO>>(_psychiatristAddress + $"/list");
        return psychiatrists;
    }

    public async Task<ICollection<PsychologistDTO>> GetPsychologistsList()
    {
        ICollection<PsychologistDTO>? psychologists = await _client
            .GetFromJsonAsync<ICollection<PsychologistDTO>>(_psychologistAddress + $"/list");
        return psychologists;
    }

    public async Task<ICollection<MedicineDTO>> GetMedicinesList()
    {
        List<MedicineDTO>? medicines = await _client
            .GetFromJsonAsync<List<MedicineDTO>>(_medicinesAddress + $"/list");

        return medicines;
    }

    public async Task<List<PatientViewModel>> GetFilteredPatients(string? searchBy, string? searchString)
    {
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        var patientsList = await response.Content.ReadFromJsonAsync<List<PatientViewModel>>();
        var matchingPatients = patientsList;
        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return patientsList;
        }

        matchingPatients = searchBy switch
        {
            (nameof(PatientViewModel.Name)) => patientsList.Where(x =>
                            (!string.IsNullOrEmpty(x.Name) ? x.Name.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PatientViewModel.Email)) => patientsList.Where(x =>
                            (!string.IsNullOrEmpty(x.Email) ? x.Email.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PatientViewModel.Address)) => patientsList.Where(x =>
                            (!string.IsNullOrEmpty(x.Address) ? x.Address.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PatientViewModel.DateOfBirth)) => patientsList.Where(x =>
                            x.DateOfBirth == null || x.DateOfBirth.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => patientsList,
        };
        return matchingPatients.ToList();
    }

    public List<PatientViewModel> GetSortedPatients(List<PatientViewModel> patients, string sortBy,
        SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return patients;
        }

        List<PatientViewModel> sortedPatients = (sortBy, sortOrder)
            switch
        {
            (nameof(PatientViewModel.Name), SortOrderOptions.ASC)
            => patients.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PatientViewModel.Name), SortOrderOptions.DESC)
            => patients.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PatientViewModel.LastName), SortOrderOptions.ASC)
            => patients.OrderBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PatientViewModel.LastName), SortOrderOptions.DESC)
            => patients.OrderByDescending(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PatientViewModel.Email), SortOrderOptions.ASC)
            => patients.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PatientViewModel.Email), SortOrderOptions.DESC)
            => patients.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            _ => patients
        };

        return sortedPatients;
    }

    public async Task<ICollection<MedicineDTO>>? GetMedicinesAssigned(int id)
    {
        ICollection<MedicineDTO>? medicinesAssigned = await
            _client.GetFromJsonAsync<ICollection<MedicineDTO>>(_baseAddress + $"/{id}/medicines");

        return medicinesAssigned ?? new List<MedicineDTO>();
    }
}