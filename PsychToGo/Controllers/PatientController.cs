
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;
using PsychToGo.Repository;

namespace PsychToGo.Controllers;



[Route( "api/[controller]" )]
[ApiController]

public class PatientController : Controller
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPsychologistRepository _psychologistRepository;
    private readonly IPsychiatristRepository _psychiatristRepository;
    private readonly IMapper _mapper;


    public PatientController(IPatientRepository patientRepository, IMapper mapper, IPsychiatristRepository psychiatristRepository, IPsychologistRepository psychologistRepository)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
        _psychiatristRepository = psychiatristRepository;
        _psychologistRepository = psychologistRepository;
    }



    [HttpGet("{id}/psychiatrist")]
    [ProducesResponseType(StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPatientPsychiatristId(int id)
    {
        var psychiatristId = await _patientRepository.GetPatientPsychiatristId( id );
        if(psychiatristId == 0)
        {
            return NotFound();
        }

        return Ok(psychiatristId);
    }

    [HttpGet( "{id}/psychologist" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPatientPsychologistId(int id)
    {
        var psychologistId = await _patientRepository.GetPatientPsychologistId( id );
        if (psychologistId == 0)
        {
            return NotFound();
        }

        return Ok( psychologistId );
    }


    [HttpGet( "patients" )]
    [ResponseCache( CacheProfileName = "Cache60" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<Patient> ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _patientRepository.GetPatients();
        if (patients == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok( _mapper.Map<List<PatientDTO>>( patients ) );
    }



    [HttpGet( "{id}" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( Patient ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPatientById(int id)
    {
        if (!await _patientRepository.PatientExists( id ))
        {
            return NotFound();
        }



        var patient = await (_patientRepository.GetPatientById( id ));
        if (patient == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        return Ok( _mapper.Map<PatientDTO>( patient ) );
    }


    [HttpGet( "{name}/patient" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( Patient ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType(StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetPatientByName(string name)
    {
        if (name.IsNullOrEmpty())
        {
            return NotFound( );
        }

        var patient = await _patientRepository.GetPatientByName( name );
        if (patient == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok( _mapper.Map<PatientDTO>( patient ) );
    }


    [HttpGet( "{id}/medicines" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<Medicine> ) )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> GetPatientMedicines(int id)
    {
        if (!await _patientRepository.PatientExists( id ))
        {
            return NotFound();
        }

        var patientMedicines = await _patientRepository.GetPatientMedicines( id );
        if (patientMedicines == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState );
        }


        return Ok( _mapper.Map<List<MedicineDTO>>( patientMedicines ) );
    }



    [HttpPost( "createNOPSYCH" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status201Created )]
    [ProducesResponseType( StatusCodes.Status422UnprocessableEntity )]

    public async Task<IActionResult> CreatePatientWithoutPsychiatrist(int psychologistId, [FromBody] PatientDTO newPatient)
    {
        if (newPatient == null)
        {
            return BadRequest( ModelState );
        }

        if (await _patientRepository.CheckDuplicate( newPatient ))
        {
            ModelState.AddModelError( "Error", "Patient already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError( "Error", "Error has occured while creating" );
            return BadRequest( ModelState );
        }

        var patientMap = _mapper.Map<Patient>( newPatient );

       
        patientMap.Psychologist = await _psychologistRepository.GetPsychologist( psychologistId );


        if (!await _patientRepository.CreatePatientWithoutPsychiatrist(patientMap ))
        {
            ModelState.AddModelError( "", "Something went wrong while saving patient." );
            return StatusCode( 500, ModelState );
        }

        return Created( "Successfully created patient", newPatient );

    }



    [HttpPost( "create" )]       
    [ProducesResponseType( StatusCodes.Status400BadRequest )]  
    [ProducesResponseType( StatusCodes.Status201Created )]
    [ProducesResponseType( StatusCodes.Status422UnprocessableEntity )]

    public async Task<IActionResult> CreatePatient(int psychologistId, int psychiatristId, int medicineId, [FromBody] PatientDTO newPatient)
    {
        if (newPatient == null)
        {
            return BadRequest( ModelState );
        }

        if (await _patientRepository.CheckDuplicate( newPatient ))
        {
            ModelState.AddModelError( "Error", "Patient already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState );
        }

        var patientMap = _mapper.Map<Patient>( newPatient );

        patientMap.Psychiatrist = await _psychiatristRepository.GetPsychiatrist( psychiatristId );
        patientMap.Psychologist = await _psychologistRepository.GetPsychologist( psychologistId );


        if (!await _patientRepository.CreatePatient( medicineId, patientMap ))
        {
            ModelState.AddModelError( "", "Something went wrong while saving patient." );
            return StatusCode( 500, ModelState );
        }

        return Created( "Successfully created patient",newPatient );

    }


    [HttpPut( "{patientId}" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> UpdatePatient(
        [FromQuery] int psychiatristId,
        [FromQuery] int psychologistId,
        [FromQuery] int medicineId,
        int patientId,
        [FromBody] PatientDTO updatedPatient)



    {
        if (updatedPatient == null)
        {
            return BadRequest( ModelState );

        }
        if (patientId != updatedPatient.Id)
        {
            return BadRequest( ModelState );
        }

        if (!await _patientRepository.PatientExists( patientId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var patient = _mapper.Map<Patient>( updatedPatient );
        patient.Psychiatrist = await _psychiatristRepository.GetPsychiatrist( psychiatristId );
        patient.Psychologist = await _psychologistRepository.GetPsychologist( psychologistId );

        if (!await _patientRepository.UpdatePatient( psychologistId, psychiatristId, medicineId, patient ))
        {
            ModelState.AddModelError( "Error", "Something went wrong updating category" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }


    [HttpDelete( "{patientId}" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> DeletePatient(int patientId)
    {
        if (!await _patientRepository.PatientExists( patientId ))
        {
            return NotFound();
        }

        var patientToDelete = await _patientRepository.GetPatientById( patientId );
        if (patientToDelete == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        if (!await _patientRepository.DeletePatient( patientToDelete ))
        {
            ModelState.AddModelError( "Error", "Something went wrong when deleting patient." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

}

