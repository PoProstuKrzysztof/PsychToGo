using PsychToGo.API.DTO;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PsychologistService : IPsychologistService
{
    private readonly Uri _baseAddress = new( "https://localhost:7291/api/Psychologist" );
    private readonly HttpClient client = new();

    public PsychologistService()
    {
        client.BaseAddress = _baseAddress;
    }

    public async Task<List<PsychologistDTO>> GetFilteredPsychologist(string searchBy, string searchString)
    {
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/list" ).Result;
        var psychologistList = await response.Content.ReadFromJsonAsync<List<PsychologistDTO>>();
        var matchingPsychologists = psychologistList;
        if (string.IsNullOrEmpty( searchBy ) || string.IsNullOrEmpty( searchString ))
        {
            return psychologistList;
        }

        switch (searchBy)
        {
            case (nameof( PsychologistDTO.Name )):
                matchingPsychologists = psychologistList.Where( x =>
                (!string.IsNullOrEmpty( x.Name ) ? x.Name.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychologistDTO.Email )):
                matchingPsychologists = psychologistList.Where( x =>
                (!string.IsNullOrEmpty( x.Email ) ? x.Email.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychologistDTO.Address )):
                matchingPsychologists = psychologistList.Where( x =>
                (!string.IsNullOrEmpty( x.Address ) ? x.Address.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychologistDTO.DateOfBirth )):
                matchingPsychologists = psychologistList.Where( x =>
                x.DateOfBirth == null || x.DateOfBirth.ToString( "dd MMMM yyyy" ).Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) ).ToList();
                break;

            default: matchingPsychologists = psychologistList; break;
        }
        return matchingPsychologists.ToList();
    }
}