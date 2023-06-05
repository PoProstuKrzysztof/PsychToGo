using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PsychToGo.API.DTO;

public record MedicineDTO
{
    [HiddenInput]
    public int Id { get; set; }

    [Required(ErrorMessage = "You have to enter valid name of the product")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Medicine name too short")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "You have to enter valid description")]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "You have to enter valid production date")]
    [DataType(DataType.Date)]
    public DateTime ProductionDate { get; set; }

    [Required(ErrorMessage = "You have to enter valid expire date")]
    [DataType(DataType.Date)]
    public DateTime ExpireDate { get; set; }

    [Required(ErrorMessage = "You have to enter valid daily dose")]
    [Range(1, 5)]
    public int DailyDose { get; set; }

    [Required(ErrorMessage = "You have to enter valid ingredients")]
    public string Ingredients { get; set; } = string.Empty;

    [Required(ErrorMessage = "You have to enter valid in stock value")]
    public int InStock { get; set; }

    public int CategoryId { get; set; }
}