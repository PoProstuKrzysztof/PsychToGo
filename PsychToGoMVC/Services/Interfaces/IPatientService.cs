using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreatePatientViewModel(PatientViewModel pvm);

    Task<PatientViewModel> CreateParsedPatientViewModel(int id);

    Task<ICollection<PsychologistDTO>> GetPsychologistsList();

    Task<ICollection<PsychiatristDTO>> GetPsychiatristsList();

    Task<ICollection<MedicineDTO>> GetMedicinesList();

    Task<ICollection<MedicineDTO>>? GetMedicinesAssigned(int id);

    Task<List<PatientViewModel>> GetFilteredPatients(string searchBy,
        string searchString);

    List<PatientViewModel> GetSortedPatients(List<PatientViewModel> patients, string sortBy, SortOrderOptions sortOrder);
}