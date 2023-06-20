using PsychToGo.API.DTO;

namespace PsychToGo.Client.Services.Interfaces;

public interface IMedicineService
{
    Task<List<MedicineDTO>> GetFilteredMedicines(string searchBy,
 string searchString);

    Task<MedicineCategoryDTO> GetMedicineCategoryById(int id);
}