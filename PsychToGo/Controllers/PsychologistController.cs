﻿using Microsoft.AspNetCore.Mvc;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PsychologistController : Controller
{
	private readonly IPsychologistRepository _psychologistRepository;

	public PsychologistController(IPsychologistRepository psychologistRepository)
	{
		_psychologistRepository= psychologistRepository;
	}



	[HttpGet]
	[ProducesResponseType(200,Type = typeof(ICollection<Psychologist>))]
	public async Task<IActionResult> GetPsychologists()
	{
		var psychologists =await _psychologistRepository.GetPsychologists();
		if(!ModelState.IsValid)
		{
			return BadRequest();
		}

		return Ok(psychologists);
	}

}
