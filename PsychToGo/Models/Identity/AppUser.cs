using Microsoft.AspNetCore.Identity;

namespace PsychToGo.API.Models.Identity;

public class AppUser : IdentityUser
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
}