using PsychToGo.Models;
using PsychToGoMVC.Models;

namespace PsychToGoMVC.Services.Interfaces;

public interface IPatientService
{
    Patient CreatePatientInstance(PatientViewModel pvm);

    Task<PatientViewModel> CreateParsedPatientInstance(int id);
}
