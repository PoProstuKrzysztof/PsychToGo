namespace PsychToGo.Models;

public class MedicineCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    //Relationships
    public virtual ICollection<Medicine> Medicines { get; set; }
}