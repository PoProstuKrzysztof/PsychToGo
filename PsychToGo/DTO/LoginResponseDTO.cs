using PsychToGo.Models;
using PsychToGo.Models.Identity;

namespace PsychToGo.DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }   
    public string Token { get; set; }
}
