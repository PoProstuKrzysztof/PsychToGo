using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPsychologistRepository
{
    //Get
    Task<ICollection<Psychologist>> GetPsychologists();

    Task<Psychologist> GetPsychologist(int id);

    Task<bool> PsychologistExist(int id);

    Task<ICollection<Patient>> GetPsychologistPatients(int id);

    //Post
    Task<bool> CreatePsychologist(Psychologist psychologist);

    Task<bool> Save();

    Task<bool> CheckDuplicate(PsychologistDTO psychologist);

    //Put
    Task<bool> UpdatePsychologist(Psychologist psychologist);

    //Delete
    Task<bool> DeletePsychologist(Psychologist psychologist);
}