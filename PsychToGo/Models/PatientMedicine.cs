namespace PsychToGo.Models;

public class PatientMedicine
{
    public int PatientId { get; set; }
    public int MedicineId { get; set; }

    public Patient Patient { get; set; }
    public Medicine Medicine { get; set; }
}