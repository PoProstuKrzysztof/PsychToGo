using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPsychologistRepository
{
    Task<ICollection<Psychologist>> GetPsychologists();
}
