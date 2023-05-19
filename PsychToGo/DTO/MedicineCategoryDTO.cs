namespace PsychToGo.API.DTO;

public record MedicineCategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}