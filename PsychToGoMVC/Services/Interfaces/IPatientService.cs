using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGo.Repository;
using PsychToGoMVC.Models;

namespace PsychToGoMVC.Services.Interfaces;

public interface IPatientService
{
    Patient CreatePatientInstance(PatientViewModel pvm);

    Task<PatientViewModel> CreateParsedPatientInstance(int id);

    Task<ICollection<PsychologistDTO>> PsychologistsList();
    Task<ICollection<PsychiatristDTO>> PsychiatristsList();
    Task<ICollection<MedicineDTO>> MedicinesList();

}
