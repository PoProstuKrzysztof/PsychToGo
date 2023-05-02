using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PsychToGo.API.DTO;

public class LoginRequestDTO
{
    [Required]
    [PersonalData]
    [EmailAddress( ErrorMessage = "Invalid e-mail" )]
    public string? UserName { get; set; }

    [Required]
    [PersonalData]
    [DataType( DataType.Password )]
    public string? Password { get; set; }
}