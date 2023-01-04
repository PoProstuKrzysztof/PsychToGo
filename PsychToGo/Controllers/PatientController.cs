
using Microsoft.AspNetCore.Mvc;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;



[Route( "api/[controller]" )]
[ApiController]
public class PatientController : Controller
{
	private readonly IPatientRepository _patientRepository;
	public PatientController(IPatientRepository patientRepository)
	{
		_patientRepository= patientRepository;
	}



	[HttpGet]
	[ProducesResponseType(200, Type = typeof(ICollection<Patient>))]
	public async Task<IActionResult> GetPatients()
	{
		var patients = await _patientRepository.GetPatients();
		if(!ModelState.IsValid)
		{
			return BadRequest();
		}
		
		return Ok( patients );
	}
}

