using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPsychologistRepository
{
    Task<ICollection<Psychologist>> GetPsychologists();
    Task<Psychologist> GetPsychologist(int id);
    Task<Psychologist> GetPsychologist(string name);
    Task<bool> PsychologistExist(int id);
    Task<ICollection<Patient>> GetPsychologistPatients(int id);

}
