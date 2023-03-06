using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PsychToGo.DTO;

public class RegistrationRequestDTO
{
    [Required(ErrorMessage = "E-mail can't be blank")]
    [EmailAddress(ErrorMessage = "E-mail should be in a proper format")]
    public string? UserName { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [DataType( DataType.Text )]
    public string? Name { get; set; }
    
    [Required]
    [StringLength( 50, MinimumLength = 3 )]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }
    
    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    [StringLength(50, MinimumLength = 5)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Password can't be blank")]
    [Compare("Password", ErrorMessage = "{0} and {1} do not match")]
    [Display(Name = "Re-enter password")]
    public string? ConfirmPassowrd { get; set; }

    [Required]
    public string? Role { get; set; }

}
