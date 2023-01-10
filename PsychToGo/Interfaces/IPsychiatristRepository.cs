using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPsychiatristRepository
{
    //Get
    Task<ICollection<Psychiatrist>> GetPsychiatrists();
    Task<Psychiatrist> GetPsychiatrist(int id);
    Task<ICollection<Patient>> GetPsychiatristPatients(int id);
    Task<bool> PsychiatristExist(int id);

    //Post
    Task<bool> CreatePsychiatrist(Psychiatrist psychiatrist);
    Task<bool> Save();
    Task<bool> CheckDuplicate(PsychiatristDTO psychiatrist);
}
