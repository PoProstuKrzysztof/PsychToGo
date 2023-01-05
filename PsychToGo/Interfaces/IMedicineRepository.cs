using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IMedicineRepository
{
    Task<ICollection<Medicine>> GetMedicines();
}
