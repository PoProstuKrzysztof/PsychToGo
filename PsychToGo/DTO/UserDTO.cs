namespace PsychToGo.API.DTO;

public record UserDTO
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }
}