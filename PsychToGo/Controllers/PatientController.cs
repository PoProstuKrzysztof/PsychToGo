
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;



[Route( "api/[controller]" )]
[ApiController]
public class PatientController : Controller
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public PatientController(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }



    [HttpGet]
    [ProducesResponseType( 200, Type = typeof( ICollection<Patient> ) )]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await _patientRepository.GetPatients();
        if (!ModelState.IsValid)
        {
            return NotFound(); 
        }

        return Ok( _mapper.Map<List<PatientDTO>>(patients) );
    }



    [HttpGet( "{id}" )]
    [ProducesResponseType( 200, Type = typeof( Patient ) )]
    [ProducesResponseType( 400 )]
    public async Task<IActionResult> GetPatient(int id)
    {
        if (!await _patientRepository.PatientExists( id ))
        {
            return NotFound();
        }



        var patient = await (_patientRepository.GetPatient( id ));
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }


        return Ok( _mapper.Map<PatientDTO>(patient) );
    }


    //[HttpGet( "{string}" )]
    //[ProducesResponseType( 200, Type = typeof( Patient ) )]
    //[ProducesResponseType( 400 )]
    //public async Task<IActionResult> GetPatientByName(string name)
    //{
    //    var patient = await _patientRepository.GetPatient( name );
    //    if(!ModelState.IsValid)
    //    {
    //        return BadRequest();
    //    }

    //    return Ok( patient );
    //}


    //[HttpGet( "{id}/medicines" )]
    //[ProducesResponseType( 200, Type = typeof( ICollection<Medicine> ) )]
    //[ProducesResponseType( 400 )]
    //public async Task<IActionResult> GetPatientMedicines(int id)
    //{
    //    if(! await _patientRepository.PatientExists(id))
    //    {
    //        return NotFound();
    //    }

    //    var patientMedicines = await _patientRepository.GetPatientMedicines( id );
    //    if(!ModelState.IsValid)
    //    {
    //        return BadRequest();
    //    }


    //    return Ok( patientMedicines );
    //}

}

