using System.ComponentModel.DataAnnotations;

namespace PsychToGo.DTO;

public class RegistrationRequestDTO
{
    [Required(ErrorMessage = "E-mail can't be blank")]
    [EmailAddress(ErrorMessage = "E-mail should be in a proper format")]
    public string UserName { get; set; }
    [Required]
    public string Name { get; set; }
    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    
    public string Password { get; set; }
    
}
