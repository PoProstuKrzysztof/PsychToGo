using Microsoft.AspNetCore.Identity;

namespace PsychToGo.Models.Identity;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
}
