using PsychToGo.API.DTO;
using PsychToGo.API.Models;

namespace PsychToGo.API.Interfaces;

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

    //Put
    Task<bool> UpdatePsychiatrist(Psychiatrist psychiatrist);

    //Delete
    Task<bool> DeletePsychiatrist(Psychiatrist psychiatrist);
}