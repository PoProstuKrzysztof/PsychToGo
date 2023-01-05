using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPatientRepository
{

   Task<ICollection<Patient>> GetPatients();
    Task<Patient> GetPatient(int id);
    Task<Patient> GetPatient(string name);
    Task<ICollection<Medicine>?> GetPatientMedicines(int id);
    Task<bool> PatientExists(int id);   
}
