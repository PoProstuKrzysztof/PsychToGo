using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using System.ComponentModel.DataAnnotations;

namespace PsychToGoMVC.Models;

public class PatientViewModel
{
    //Basic information
    [HiddenInput]
    public int Id { get; set; }

    [Required]
    [StringLength( 30, MinimumLength = 3, ErrorMessage = "Last name is too short!" )]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength( 30, MinimumLength = 3, ErrorMessage = "Last name is too short!" )]
    public string LastName { get; set; } = string.Empty;

    [Required( ErrorMessage = "The e-mail adress is required" )]
    [RegularExpression( @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$" )]
    [EmailAddress( ErrorMessage = "Invalid email address" )]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone( ErrorMessage = "Invalid phone number" )]
    [RegularExpression( "^[0-9]*$", ErrorMessage = "Wrong phone number" )]
    [StringLength( 12, MinimumLength = 9, ErrorMessage = "Phone number is too short!" )]
    public string Phone { get; set; } = string.Empty;

    [Required( ErrorMessage = "You have to enter valid date of birth!" )]
    [DataType( DataType.Date )]
    public DateTime DateOfBirth { get; set; }

    [Required( ErrorMessage = "You have to enter valid address!" )]
    [StringLength( 30, MinimumLength = 4 )]
    public string Address { get; set; } = string.Empty;

    // Information about his assignments
    [Required( ErrorMessage = "You have to choose psychologist" )]
    public int PsychologistId { get; set; }

    public virtual ICollection<PsychologistDTO>? Psychologists { get; set; }

    public List<int>? MedicinesId { get; set; }
    public virtual ICollection<MedicineDTO>? Medicines { get; set; }

    public int? PsychiatristId { get; set; }
    public virtual ICollection<PsychiatristDTO>? Psychiatrists { get; set; }
}