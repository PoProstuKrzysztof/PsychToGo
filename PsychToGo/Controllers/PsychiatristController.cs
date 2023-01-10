using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class PsychiatristController : Controller
{
    private readonly IPsychiatristRepository _psychiatristRepository;
    private readonly IMapper _mapper;
    public PsychiatristController(IPsychiatristRepository psychiatristRepository, IMapper mapper)
    {
        _psychiatristRepository = psychiatristRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType( 200, Type = typeof( ICollection<Psychiatrist> ) )]
    public async Task<IActionResult> GetPsychiatrists()
    {
        var psychiatrists = await _psychiatristRepository.GetPsychiatrists();
        if (psychiatrists == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return BadRequest();
        }


        return Ok( _mapper.Map<List<PsychiatristDTO>>( psychiatrists ) );

    }


    [HttpGet( "{id}" )]
    [ProducesResponseType( 200, Type = typeof( Psychiatrist ) )]
    [ProducesResponseType( 400 )]
    public async Task<IActionResult> GetPsychiatrist(int id)
    {
        if (!await _psychiatristRepository.PsychiatristExist( id ))
        {
            return NotFound();
        }

        var psychiatrist = await _psychiatristRepository.GetPsychiatrist( id );
        if (psychiatrist == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok( _mapper.Map<PsychiatristDTO>( psychiatrist ) );
    }


    [HttpGet("{id}/patients")]
    [ProducesResponseType(200,Type = typeof(ICollection<Patient>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPsychiatrisPatients(int id)
    {
        try
        {
            if(! await _psychiatristRepository.PsychiatristExist(id))
            {
                return NotFound();
            }

            var psychatirstPatients = await _psychiatristRepository.GetPsychiatristPatients( id );
            if (psychatirstPatients == null)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<List<PatientDTO>>( psychatirstPatients ) );
        }
        catch(Exception)
        {
            throw;
        }
    }

}
