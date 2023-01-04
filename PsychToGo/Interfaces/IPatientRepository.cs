using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPatientRepository
{

   Task<ICollection<Patient>> GetPatients();
}
