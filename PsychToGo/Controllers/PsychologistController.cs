using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class PsychologistController : Controller
{
    private readonly IPsychologistRepository _psychologistRepository;
    private readonly IMapper _mapper;

    public PsychologistController(IPsychologistRepository psychologistRepository, IMapper mapper)
    {
        _psychologistRepository = psychologistRepository;
        _mapper = mapper;
    }

    //Get

    [HttpGet( "list" )]
    [ResponseCache( CacheProfileName = "Cache60" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<Psychologist> ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetAllPsychologists()
    {
        ICollection<Psychologist> psychologists = await _psychologistRepository.GetPsychologists();
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok( _mapper.Map<List<PsychologistDTO>>( psychologists ) );
    }

    [HttpGet( "{id}" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( Psychologist ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPsychologist(int id)
    {
        if (!await _psychologistRepository.PsychologistExist( id ))
        {
            return NotFound();
        }

        Psychologist psychologist = await _psychologistRepository.GetPsychologist( id );
        if (psychologist == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        return Ok( _mapper.Map<PsychologistDTO>( psychologist ) );
    }

    [HttpGet( "{psychologistId}/patients" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPsychologistPatients(int psychologistId)
    {
        if (!await _psychologistRepository.PsychologistExist( psychologistId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        ICollection<Patient> psychologistPatients = await _psychologistRepository.GetPsychologistPatients( psychologistId );
        if (psychologistPatients == null)
        {
            return NotFound();
        }
        return Ok( psychologistPatients );
    }

    //Post
    [HttpPost( "create" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status201Created )]
    public async Task<IActionResult> CreatePsychologist([FromBody] PsychologistDTO newPsychologist)
    {
        if (newPsychologist == null)
        {
            return BadRequest( ModelState );
        }

        if (await _psychologistRepository.CheckDuplicate( newPsychologist ))
        {
            ModelState.AddModelError( "", "Psychologist already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        Psychologist psychologist = _mapper.Map<Psychologist>( newPsychologist );
        if (!await _psychologistRepository.CreatePsychologist( psychologist ))
        {
            ModelState.AddModelError( "Error", "Something went wrong while saving psychologist." );
            return StatusCode( 500, ModelState );
        }

        return Created( "Successfully created psychologist", psychologist );
    }

    //Put
    [HttpPut( "{psychologistId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> UpdatePsychologist(int psychologistId, [FromBody] PsychologistDTO updatedPsychologist)

    {
        if (updatedPsychologist == null)
        {
            return NotFound();
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
            return BadRequest( ModelState );
        }

        Psychologist psychologist = _mapper.Map<Psychologist>( updatedPsychologist );

        if (!await _psychologistRepository.UpdatePsychologist( psychologist ))
        {
            ModelState.AddModelError( "Error", "Something went wrong updating psychologist" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

    //Delte
    [HttpDelete( "{psychologistId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> DeletePsychologist(int psychologistId)
    {
        if (!await _psychologistRepository.PsychologistExist( psychologistId ))
        {
            return NotFound();
        }

        Psychologist psychologistToDelete = await _psychologistRepository.GetPsychologist( psychologistId );
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
            ModelState.AddModelError( "Error", "Something went wrong when deleting psychologist." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }
}