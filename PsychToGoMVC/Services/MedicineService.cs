using PsychToGo.API.DTO;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class MedicineService : IMedicineService
{
    private readonly Uri _baseAddress = new("https://localhost:7291/api/Medicine");
    private readonly Uri _medicineCategory = new("https://localhost:7291/api/MedicineCategory");
    private readonly HttpClient _client = new();

    public MedicineService()
    {
        _client.BaseAddress = _baseAddress;
    }

    public async Task<List<MedicineDTO>> GetFilteredMedicines(string searchBy, string searchString)
    {
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        var medicinesList = await response.Content.ReadFromJsonAsync<List<MedicineDTO>>();
        var matchingMedicines = medicinesList;
        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return medicinesList;
        }

        matchingMedicines = searchBy switch
        {
            (nameof(MedicineDTO.Name)) => medicinesList.Where(x =>
                            (!string.IsNullOrEmpty(x.Name) ? x.Name.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(MedicineDTO.ProductionDate)) => medicinesList.Where(x =>
                            x.ProductionDate == null || x.ProductionDate.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase)).ToList(),
            (nameof(MedicineDTO.InStock)) => medicinesList.Where(x =>
                            x.InStock == null || x.InStock.ToString()
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(),
            (nameof(MedicineDTO.ExpireDate)) => medicinesList.Where(x =>
                            x.ExpireDate == null || x.ExpireDate.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => medicinesList,
        };
        return matchingMedicines.ToList();
    }

    public async Task<MedicineCategoryDTO> GetMedicineCategoryById(int id)
    {
        var medicineCategory = await _client.GetFromJsonAsync<MedicineCategoryDTO>(_medicineCategory + $"/medicine/{id}");

        return medicineCategory;
    }
}