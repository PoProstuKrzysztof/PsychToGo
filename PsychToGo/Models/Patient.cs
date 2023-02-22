using Newtonsoft.Json;

namespace PsychToGo.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;

    //Relationships
    
    public virtual ICollection<PatientMedicine>? PatientMedicines { get; set; }
    public  int PsychologistId { get; set; }
    public Psychologist? Psychologist { get; set; }
    public  int? PsychiatristId { get; set; }
    public Psychiatrist? Psychiatrist { get; set; }
}