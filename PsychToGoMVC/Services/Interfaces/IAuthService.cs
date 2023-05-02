using PsychToGo.API.DTO;

namespace PsychToGo.Client.Services.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync<T>(LoginRequestDTO userLogin);

    Task<string> RegisterAsync<T>(RegistrationRequestDTO userCreate);
}