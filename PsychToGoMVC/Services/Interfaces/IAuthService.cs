
using PsychToGo.DTO;

namespace PsychToGoMVC.Services.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync<T>(LoginRequestDTO userLogin);
    Task<string> RegisterAsync<T>(RegistrationRequestDTO userCreate);
}
