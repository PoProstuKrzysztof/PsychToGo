using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PsychToGo.API.DTO;

public record PsychologistDTO
{
    [HiddenInput]
    public int Id { get; set; }

    [Required( ErrorMessage = "You have to enter valid name!" )]
    public string Name { get; set; } = string.Empty;

    [Required( ErrorMessage = "You have to enter valid last name!" )]
    public string LastName { get; set; } = string.Empty;

    [Required( ErrorMessage = "You have to enter valid e-mail!" )]
    [RegularExpression( @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$" )]

    [EmailAddress( ErrorMessage = "Invalid email address" )]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone( ErrorMessage = "Invalid phone number" )]
    [StringLength( 12, MinimumLength = 9, ErrorMessage = "Phone number is too short!" )]
    public string Phone { get; set; } = string.Empty;

    [Required( ErrorMessage = "You have to enter valid birth date!" )]
    [DataType( DataType.Date )]
    public DateTime DateOfBirth { get; set; }

    [Required( ErrorMessage = "You have to enter valid address! " )]
    public string Address { get; set; } = string.Empty;
}