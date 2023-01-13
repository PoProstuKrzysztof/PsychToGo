﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;
using PsychToGo.Repository;

namespace PsychToGo.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PsychologistController : Controller
{
	private readonly IPsychologistRepository _psychologistRepository;
	private readonly IMapper _mapper;

	public PsychologistController(IPsychologistRepository psychologistRepository, IMapper mapper)
	{
		_psychologistRepository= psychologistRepository;
		_mapper= mapper;
	}

	//Get

	[HttpGet( "list" )]
	[ProducesResponseType(200,Type = typeof(ICollection<Psychologist>))]
	public async Task<IActionResult> GetAllPsychologists()
	{
		var psychologists =await _psychologistRepository.GetPsychologists();
		if(!ModelState.IsValid)
		{
			return BadRequest();
		}

		return Ok(_mapper.Map<List<PsychologistDTO>>(psychologists));
	}


	[HttpGet("{id}")]
	[ProducesResponseType(200, Type = typeof(Psychologist))]
    [ProducesResponseType( 400)]
    public async Task<IActionResult> GetPsychologist(int id)
	{
		if(! await _psychologistRepository.PsychologistExist(id))
		{
			return NotFound();
		}

		var psychologist = await _psychologistRepository.GetPsychologist( id );
		if (psychologist == null)
		{
			return NotFound();
		}

		if(!ModelState.IsValid)
		{
			return BadRequest();
		}

		return Ok(_mapper.Map<PsychologistDTO>(psychologist));
	}

	[HttpGet( "{psychologistId}/patients" )]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetPsychologistPatients(int psychologistId)
	{
		if(!await _psychologistRepository.PsychologistExist(psychologistId))
		{
			return NotFound();
		}

		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var psychologistPatients = await _psychologistRepository.GetPsychologistPatients(psychologistId);
		if(psychologistPatients == null)
		{
			return NotFound();
		}
		return Ok( _mapper.Map<List<PatientDTO>>( psychologistPatients ) );
	}
	//Post
	[HttpPost( "create" )]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreatePsychologist([FromBody] PsychologistDTO newPsychologist)
	{
		if(newPsychologist == null)
		{
			return BadRequest(ModelState);
		}

		if(await _psychologistRepository.CheckDuplicate(newPsychologist))
		{
			ModelState.AddModelError( "", "Psychologist already exists." );
			return StatusCode(422,ModelState);
		}

		if(!ModelState.IsValid)
		{
			return BadRequest();
		}

		var psychologist = _mapper.Map<Psychologist>(newPsychologist);
		if(! await _psychologistRepository.CreatePsychologist(psychologist))
		{
			ModelState.AddModelError( "", "Something went wrong while saving psychologist." );
			return StatusCode(500,ModelState);
		}

		return Ok( "Successfully created psychologist" );

	}

	//Put
    [HttpPut( "{psychologistId}" )]
    [ProducesResponseType( 204 )]
    [ProducesResponseType( 400 )]
    [ProducesResponseType( 404 )]
    public async Task<IActionResult> UpdatePsychologist (int psychologistId,[FromBody] PsychologistDTO updatedPsychologist)

    {
        if (updatedPsychologist == null)
        {
            return BadRequest( ModelState );

        }
        if (psychologistId != updatedPsychologist.Id)
        {
            return BadRequest( ModelState );
		}

        if (!await _psychologistRepository.PsychologistExist( psychologistId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var psychologist = _mapper.Map<Psychologist>( updatedPsychologist );       

        if (!await _psychologistRepository.UpdatePsychologist( psychologist ))
        {
            ModelState.AddModelError( "", "Something went wrong updating psychologist" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }


	//Delte
    [HttpDelete( "{psychologistId}" )]
    [ProducesResponseType( 400 )]
    [ProducesResponseType( 204 )]
    [ProducesResponseType( 404 )]
    public async Task<IActionResult> DeletePsychologist(int psychologistId)
    {
        if (!await _psychologistRepository.PsychologistExist( psychologistId ))
        {
            return NotFound();
        }

        var psychologistToDelete = await _psychologistRepository.GetPsychologist( psychologistId );
        if (psychologistToDelete == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        if (!await _psychologistRepository.DeletePsychologist( psychologistToDelete ))
        {
            ModelState.AddModelError( "", "Something went wrong when deleting psychologist." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }


}
