using Microsoft.AspNetCore.Mvc;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PsychiatristController : Controller
{
    private readonly IPsychiatristRepository _psychiatristRepository;

    public PsychiatristController(IPsychiatristRepository psychiatristRepository)
    {
        _psychiatristRepository= psychiatristRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ICollection<Psychiatrist>))]
    public async  Task<IActionResult> GetPsychiatrists()
    {
        var psychiatrists = await _psychiatristRepository.GetPsychiatrists();

        if(!ModelState.IsValid)
        {
            return BadRequest();
        }


        return Ok(psychiatrists);

    }
}
