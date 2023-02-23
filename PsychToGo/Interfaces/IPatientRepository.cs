using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPatientRepository
{
    //Get
    Task<ICollection<Patient>> GetPatients();
    Task<Patient> GetPatientById(int id);
    Task<Patient> GetPatientByName(string name);
    Task<ICollection<Medicine>?> GetPatientMedicines(int id);
    Task<bool> PatientExists(int id); 
    Task<int?> GetPatientPsychiatristId(int id);
    Task<int?> GetPatientPsychologistId(int id);

    //Post
    Task<bool> CreatePatient(int medicineId, Patient patient);
    Task<bool> CreatePatientWithoutPsychiatrist(Patient patient);
    Task<bool> Save();
    Task<bool> CheckDuplicate(PatientDTO patient);

    //Put
    Task<bool> UpdatePatient(int psychologistId,int psychiatristId,int medicineId, Patient patient);

    //Delete
    Task<bool> DeletePatient(Patient patient);


}
