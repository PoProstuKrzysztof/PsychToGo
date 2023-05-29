using PsychToGo.API.DTO;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PsychiatristService : IPsychiatristService
{
    private readonly Uri _baseAddress = new("https://localhost:7291/api/Psychiatrist");
    private readonly HttpClient _client = new();

    public PsychiatristService()
    {
        _client.BaseAddress = _baseAddress;
    }

    public async Task<List<PsychiatristDTO>> GetFilteredPsychiatrists(string searchBy, string searchString)
    {
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        var psychiatristList = await response.Content.ReadFromJsonAsync<List<PsychiatristDTO>>();
        var matchingPsychiatrists = psychiatristList;
        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return psychiatristList;
        }

        matchingPsychiatrists = searchBy switch
        {
            (nameof(PsychiatristDTO.Name)) => psychiatristList.Where(x =>
                            (!string.IsNullOrEmpty(x.Name) ? x.Name.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychiatristDTO.Email)) => psychiatristList.Where(x =>
                            (!string.IsNullOrEmpty(x.Email) ? x.Email.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychiatristDTO.Address)) => psychiatristList.Where(x =>
                            (!string.IsNullOrEmpty(x.Address) ? x.Address.Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true)).ToList(),
            (nameof(PsychiatristDTO.DateOfBirth)) => psychiatristList.Where(x =>
                            x.DateOfBirth == null || x.DateOfBirth.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => psychiatristList,
        };
        return matchingPsychiatrists.ToList();
    }

    public List<PsychiatristDTO> GetSortedPsychiatrists(List<PsychiatristDTO> psychiatrists, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return psychiatrists;
        }

        List<PsychiatristDTO> sortedPsychiatrists = (sortBy, sortOrder)
            switch
        {
            (nameof(PsychiatristDTO.Name), SortOrderOptions.ASC)
            => psychiatrists.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychiatristDTO.Name), SortOrderOptions.DESC)
            => psychiatrists.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychiatristDTO.LastName), SortOrderOptions.ASC)
            => psychiatrists.OrderBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychiatristDTO.LastName), SortOrderOptions.DESC)
            => psychiatrists.OrderByDescending(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychologistDTO.Email), SortOrderOptions.ASC)
            => psychiatrists.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            (nameof(PsychiatristDTO.Email), SortOrderOptions.DESC)
            => psychiatrists.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToList(),

            _ => psychiatrists
        };

        return sortedPsychiatrists;
    }
}