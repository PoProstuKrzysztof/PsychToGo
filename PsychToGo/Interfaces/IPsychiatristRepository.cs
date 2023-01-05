using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IPsychiatristRepository
{
    Task<ICollection<Psychiatrist>> GetPsychiatrists();
}
