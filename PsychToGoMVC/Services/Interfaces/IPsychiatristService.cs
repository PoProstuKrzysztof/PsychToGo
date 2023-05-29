using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPsychiatristService
{
    Task<List<PsychiatristDTO>> GetFilteredPsychiatrists(string searchBy,
       string searchString);

    List<PsychiatristDTO> GetSortedPsychiatrists(List<PsychiatristDTO> psychiatrists, string sortBy, SortOrderOptions sortOrder);
}