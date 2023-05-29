using PsychToGo.API.DTO;
using PsychToGo.Client.Enums;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPsychologistService
{
    Task<List<PsychologistDTO>> GetFilteredPsychologist(string searchBy,
     string searchString);

    List<PsychologistDTO> GetSortedPsychologist(List<PsychologistDTO> psychologists, string sortBy, SortOrderOptions sortOrder);
}