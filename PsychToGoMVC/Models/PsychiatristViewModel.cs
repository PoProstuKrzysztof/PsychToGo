using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsychToGoMVC.Models;

public class PsychiatristViewModel
{
    public int Id { get; set; } 
    [Required( ErrorMessage = "You have to enter valid name!" )]
    public string Name { get; set; } = string.Empty;
    [Required( ErrorMessage = "You have to enter valid last name!" )]
    public string LastName { get; set; } = string.Empty;
    [Required( ErrorMessage = "You have to enter valid e-mail!" )]
    [DataType( DataType.EmailAddress )]
    public string Email { get; set; } = string.Empty;
    [DataType( DataType.PhoneNumber )]
    public string Phone { get; set; } = string.Empty;
    [Required( ErrorMessage = "You have to enter valid birth date!" )]
    [DataType( DataType.Date )]
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
}
