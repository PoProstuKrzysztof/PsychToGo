using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PsychologistService : IPsychologistService
{
    private readonly Uri _baseAddress = new("https://localhost:7291/api/Psychologist");
    private readonly HttpClient _client = new();

    public PsychologistService()
    {
        _client.BaseAddress = _baseAddress;
    }

    public async Task<List<PsychologistDTO>> GetFilteredPsychologist(string searchBy, string searchString)
    {
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        var psychologistList = await response.Content.ReadFromJsonAsync<List<PsychologistDTO>>();
        var matchingPsychologists = psychologistList;
        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return psychologistList;
        }

        matchingPsychologists = searchBy switch
        {
            (nameof(PsychologistDTO.Name)) => psychologistList.Where(x =>
                            (!string.IsNullOrEmpty(x.Name) ? x.Name.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychologistDTO.Email)) => psychologistList.Where(x =>
                            (!string.IsNullOrEmpty(x.Email) ? x.Email.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychologistDTO.Address)) => psychologistList.Where(x =>
                            (!string.IsNullOrEmpty(x.Address) ? x.Address.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychologistDTO.DateOfBirth)) => psychologistList.Where(x =>
                            x.DateOfBirth == null || x.DateOfBirth.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => psychologistList,
        };
        return matchingPsychologists.ToList();
    }

    public List<PsychologistDTO> GetSortedPsychologist(List<PsychologistDTO> psychologists, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return psychologists;
        }

        List<PsychologistDTO> sortedPsychologists = (sortBy, sortOrder)
            switch
        {
            (nameof(PsychologistDTO.Name), SortOrderOptions.ASC)
            => psychologists.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.Name), SortOrderOptions.DESC)
            => psychologists.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.LastName), SortOrderOptions.ASC)
            => psychologists.OrderBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.LastName), SortOrderOptions.DESC)
            => psychologists.OrderByDescending(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.Email), SortOrderOptions.ASC)
            => psychologists.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.Email), SortOrderOptions.DESC)
            => psychologists.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            _ => psychologists
        };

        return sortedPsychologists;
    }
}