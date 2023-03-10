using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
    Task<UserDTO> Register(RegistrationRequestDTO registrationRequest);
}
