using PsychToGo.API.DTO;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPsychologistService
{
    Task<List<PsychologistDTO>> GetFilteredPsychologist(string searchBy,
     string searchString);
}