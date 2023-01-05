using Newtonsoft.Json;

namespace PsychToGo.Models;

public class Psychiatrist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;

    //Relationships
    [JsonIgnore]
    public virtual ICollection<Patient>? Patients { get; set; }
}