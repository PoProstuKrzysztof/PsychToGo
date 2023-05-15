using PsychToGo.API.DTO;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;

namespace PsychToGo.Client.Services;

public class PsychiatristService : IPsychiatristService
{
    private readonly Uri _baseAddress = new( "https://localhost:7291/api/Psychiatrist" );
    private readonly HttpClient client = new();

    public PsychiatristService()
    {
        client.BaseAddress = _baseAddress;
    }

    public async Task<List<PsychiatristDTO>> GetFilteredPsychiatrists(string searchBy, string searchString)
    {
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/psychiatrist" ).Result;
        var psychiatristList = await response.Content.ReadFromJsonAsync<List<PsychiatristDTO>>();
        var matchingPsychiatrists = psychiatristList;
        if (string.IsNullOrEmpty( searchBy ) || string.IsNullOrEmpty( searchString ))
        {
            return psychiatristList;
        }

        switch (searchBy)
        {
            case (nameof( PsychiatristDTO.Name )):
                matchingPsychiatrists = psychiatristList.Where( x =>
                (!string.IsNullOrEmpty( x.Name ) ? x.Name.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychiatristDTO.Email )):
                matchingPsychiatrists = psychiatristList.Where( x =>
                (!string.IsNullOrEmpty( x.Email ) ? x.Email.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychiatristDTO.Address )):
                matchingPsychiatrists = psychiatristList.Where( x =>
                (!string.IsNullOrEmpty( x.Address ) ? x.Address.Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) : true) ).ToList();
                break;

            case (nameof( PsychiatristDTO.DateOfBirth )):
                matchingPsychiatrists = psychiatristList.Where( x =>
                x.DateOfBirth == null || x.DateOfBirth.ToString( "dd MMMM yyyy" ).Contains( searchString,
                StringComparison.OrdinalIgnoreCase ) ).ToList();
                break;

            default: matchingPsychiatrists = psychiatristList; break;
        }
        return matchingPsychiatrists.ToList();
    }
}