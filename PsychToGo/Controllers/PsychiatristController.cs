using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;
using PsychToGo.Repository;

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

    [HttpGet( "list" )]
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


    [HttpGet( "{psychiatristId}/patients" )]
    [ProducesResponseType(200,Type = typeof(ICollection<Patient>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPsychiatrisPatients(int psychiatristId)
    {
        try
        {
            if(! await _psychiatristRepository.PsychiatristExist( psychiatristId ))
            {
                return NotFound();
            }

            var psychatirstPatients = await _psychiatristRepository.GetPsychiatristPatients( psychiatristId );
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

    [HttpPost( "create" )]
    [ProducesResponseType( 201 )]
    [ProducesResponseType( 400 )]
    public async Task<IActionResult> CreatePsychiatrist([FromBody] PsychiatristDTO newPsychiatrist)
    {
        if (newPsychiatrist == null)
        {
            return BadRequest( ModelState );
        }

        if (await _psychiatristRepository.CheckDuplicate( newPsychiatrist ))
        {
            ModelState.AddModelError( "", "Psychiatrist already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var psychiatrist = _mapper.Map<Psychiatrist>( newPsychiatrist );
        if (!await _psychiatristRepository.CreatePsychiatrist( psychiatrist ))
        {
            ModelState.AddModelError( "", "Something went wrong while saving psychiatrist." );
            return StatusCode( 500, ModelState );
        }

        return Ok( "Successfully created psychologist" );

    }

    [HttpPut( "{psychiatristId}" )]
    [ProducesResponseType( 204 )]
    [ProducesResponseType( 400 )]
    [ProducesResponseType( 404 )]
    public async Task<IActionResult> UpdatePsychiatrist(int psychiatristId, [FromBody] PsychiatristDTO updatedPsychiatrist)

    {
        if (updatedPsychiatrist == null)
        {
            return BadRequest( ModelState );

        }
        if (psychiatristId != updatedPsychiatrist.Id)
        {
            return BadRequest( ModelState );
        }

        if (!await _psychiatristRepository.PsychiatristExist( psychiatristId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var psychiatrist = _mapper.Map<Psychiatrist>( updatedPsychiatrist );

        if (!await _psychiatristRepository.UpdatePsychiatrist( psychiatrist ))
        {
            ModelState.AddModelError( "", "Something went wrong updating psychiatrist" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }



    [HttpDelete( "{psychiatristId}" )]
    [ProducesResponseType( 400 )]
    [ProducesResponseType( 204 )]
    [ProducesResponseType( 404 )]
    public async Task<IActionResult> DeletePsychatrist(int psychiatristId)
    {
        if (!await _psychiatristRepository.PsychiatristExist( psychiatristId ))
        {
            return NotFound();
        }

        var psychatristToDelete = await _psychiatristRepository.GetPsychiatrist( psychiatristId );
        if (psychatristToDelete == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        if (!await _psychiatristRepository.DeletePsychiatrist( psychatristToDelete ))
        {
            ModelState.AddModelError( "", "Something went wrong when deleting psychiatrist." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }




}
