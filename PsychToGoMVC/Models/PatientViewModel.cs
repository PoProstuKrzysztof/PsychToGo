using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsychToGoMVC.Models;

public class PatientViewModel
{

    public int Id { get; set; }
    
    [Required(ErrorMessage = "You have to enter valid name!")]
    public string Name { get; set; } = string.Empty;
   
    [Required( ErrorMessage = "You have to enter valid last name!" )]
    public string LastName { get; set; } = string.Empty;
    
    [Required( ErrorMessage = "The e-mail adress is required" )]
    [RegularExpression( @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$")]
    [EmailAddress(ErrorMessage ="Invalid email address")]
    public string Email { get; set; } = string.Empty;

    
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;
  
    [Required(ErrorMessage ="You have to enter valid birth date!")]   
    [DataType( DataType.Date )]   
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;

    public virtual int PsychologistId { get; set; }
    public virtual int MedicineId { get; set; }
    public virtual int PsychiatristId { get; set; }
}

