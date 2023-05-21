using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreatePatientInstance(PatientViewModel pvm);

    Task<PatientViewModel> CreateParsedPatientInstance(int id);

    Task<ICollection<PsychologistDTO>> PsychologistsList();

    Task<ICollection<PsychiatristDTO>> PsychiatristsList();

    Task<ICollection<MedicineDTO>> MedicinesList();

    Task<List<PatientViewModel>> GetFilteredPatients(string searchBy,
        string searchString);

    List<PatientViewModel> GetSortedPatients(List<PatientViewModel> patients, string sortBy, SortOrderOptions sortOrder);
}