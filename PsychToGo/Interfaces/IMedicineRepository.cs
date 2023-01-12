using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IMedicineRepository
{
    //Get
    Task<ICollection<Medicine>> GetMedicines();
    Task<Medicine> GetMedicine(int id);
    Task<bool> MedicineExists(int id);
    Task<int> GetMedicineInStock(int id);
    Task<DateTime> GetMedicineExpireDate(int id);

    //Post
    Task<bool> CreateMedicine(int categoryId, Medicine medicine);
    Task<bool> Save();
    Task<bool> CheckDuplicate(MedicineDTO medicine);

    //Put
    Task<bool> UpdateMedicine(int categoryId,Medicine medicine);
}
