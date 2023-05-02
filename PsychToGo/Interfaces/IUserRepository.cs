using PsychToGo.API.DTO;

namespace PsychToGo.API.Interfaces;

public interface IUserRepository
{
    bool IsUniqueUser(string username);

    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);

    Task<UserDTO> Register(RegistrationRequestDTO registrationRequest);
}