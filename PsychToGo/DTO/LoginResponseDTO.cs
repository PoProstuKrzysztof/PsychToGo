using PsychToGo.Models;

namespace PsychToGo.DTO;

public class LoginResponseDTO
{
    public LocalUser User { get; set; }
    public string Token { get; set; }
}
