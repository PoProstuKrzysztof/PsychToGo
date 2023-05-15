using PsychToGo.API.DTO;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPsychiatristService
{
    Task<List<PsychiatristDTO>> GetFilteredPsychiatrists(string searchBy,
       string searchString);
}