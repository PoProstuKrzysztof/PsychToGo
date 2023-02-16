using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PsychToGo.DTO;

public class UserDTO
{
    
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Name { get; set; }

    public string LastName { get; set; }
}
