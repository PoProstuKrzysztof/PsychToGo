using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGoMVC.Services.Interfaces;
using System.Text;

namespace PsychToGoMVC.Services;

public class AuthService : IAuthService
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/UsersAuthorize" );
    HttpClient client = new HttpClient();

    public AuthService()
    {
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }


    public async Task<string> LoginAsync<T>(LoginRequestDTO userLogin)
    {
        string data = JsonConvert.SerializeObject( userLogin );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );

        HttpResponseMessage response =  client.PostAsync( client.BaseAddress + "/login" ,content).Result;
        if(response.IsSuccessStatusCode)
        {
            string result = response.Content.ReadAsStringAsync().Result;


            return result;
        }

        return string.Empty ;

      
    }

    public async Task<string> RegisterAsync<T>(RegistrationRequestDTO userCreate)
    {
        string data = JsonConvert.SerializeObject( userCreate );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );

        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/register", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            string result = response.Content.ReadAsStringAsync().Result;


            return result;
        }

        return string.Empty;
    }
}
