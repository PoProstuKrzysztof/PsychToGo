using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.Client.Services.Interfaces;
using System.Text;

namespace PsychToGo.Client.Services;

public class AuthService : IAuthService
{
    private readonly Uri baseAdress = new("https://localhost:7291/api/UsersAuthorize");
    private readonly HttpClient client = new();

    public AuthService()
    {
        client.BaseAddress = baseAdress;
    }

    /// <summary>
    /// Login call to API
    /// </summary>
    /// <param name="userLogin"></param>
    /// <returns></returns>
    public async Task<string> LoginAsync<T>(LoginRequestDTO userLogin)
    {
        string data = JsonConvert.SerializeObject(userLogin);

        StringContent content = new(data,
                                                  Encoding.UTF8,
                                                  "application/json");

        HttpResponseMessage response = client.PostAsync(client.BaseAddress + "/login", content).Result;
        if (response.IsSuccessStatusCode)
        {
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        return string.Empty;
    }

    /// <summary>
    /// Register call to API
    /// </summary>
    /// <param name="userCreate"></param>
    /// <returns></returns>
    public async Task<string> RegisterAsync<T>(RegistrationRequestDTO userCreate)
    {
        string data = JsonConvert.SerializeObject(userCreate);

        StringContent content = new(data,
                                    Encoding.UTF8,
                                    "application/json");

        HttpResponseMessage response = client.PostAsync(client.BaseAddress + "/register",
                                                        content).Result;
        if (response.IsSuccessStatusCode)
        {
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        return string.Empty;
    }
}