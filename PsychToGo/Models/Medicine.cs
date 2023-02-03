using Newtonsoft.Json;

namespace PsychToGo.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ProductionDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public int DailyDose { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public int InStock { get; set; }

    //Relationships 
    
    public MedicineCategory Category { get; set; }
    
    public virtual ICollection<PatientMedicine?> PatientMedicines { get; set; }
}