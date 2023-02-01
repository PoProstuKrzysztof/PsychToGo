using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;
using PsychToGo.Repository;

namespace PsychToGo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineController : Controller
{
    private readonly IMedicineRepository _medicineRepository;
    private readonly IMedicineCategoryRepository _medicineCategoryRepository;
    private readonly IMapper _mapper;
    public MedicineController(IMedicineRepository medicineRepository, IMapper mapper, IMedicineCategoryRepository medicineCategoryRepository)
    {
        _medicineRepository = medicineRepository;
        _medicineCategoryRepository= medicineCategoryRepository;
        _mapper = mapper;
    }

    [HttpGet( "list" )]
    [ProducesResponseType(200,Type = typeof(ICollection<Medicine>))]
    public async Task<IActionResult> GetMedicines()
    {
        var medicines = await _medicineRepository.GetMedicines();
        if(!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(_mapper.Map<List<MedicineDTO>>( medicines ));
    }


    [HttpGet("{medicineId}/inStock")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetMedicineInStock(int medicineId)
    {
        if(!await _medicineRepository.MedicineExists(medicineId))
        {
            return NotFound();
        }

        if(!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        var inStock = await _medicineRepository.GetMedicineInStock( medicineId );
    
        return Ok(inStock);

    }

    [HttpGet("{medicineId}/ExpireDate")]
    [ProducesResponseType(200)]
    [ProducesResponseType( 400)]
    public async Task<IActionResult> GetMedicineExpireDate(int medicineId)
    {
        if(!await _medicineRepository.MedicineExists(medicineId))
        {
            return NotFound();
        }

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var medicineExpireDate = await _medicineRepository.GetMedicineExpireDate( medicineId );
        if(medicineExpireDate == null)
        {
            return NotFound();
        }

        return Ok(medicineExpireDate);

    }

    [HttpGet("{id}")]
    [ProducesResponseType(200,Type = typeof(Medicine))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetMedicineById(int id)
    {
        if(! await _medicineRepository.MedicineExists(id))
        {
            return NotFound();
        }

        var medicine = await _medicineRepository.GetMedicine(id);
        if(medicine == null)
        {
            return NotFound();
        }


        if(!ModelState.IsValid)
        {
            return BadRequest();
        }
        return Ok(_mapper.Map<MedicineDTO>(medicine));
    }


    [HttpPost( "create" )]
    [ProducesResponseType( 201 )]
    [ProducesResponseType( 400 )]
    public async Task<IActionResult> CreateMedicine([FromQuery] int categoryId, [FromBody] MedicineDTO newMedicine)
    {
        if (newMedicine == null)
        {
            return BadRequest( ModelState );
        }

        if (await _medicineRepository.CheckDuplicate( newMedicine ))
        {   
            ModelState.AddModelError( "", "Medicine already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var medicine = _mapper.Map<Medicine>( newMedicine );
        medicine.Category = await _medicineCategoryRepository.GetMedicineCategoryById( categoryId );
        if (!await _medicineRepository.CreateMedicine(newMedicine.CategoryId, medicine ))
        {
            ModelState.AddModelError( "", "Something went wrong while saving medicine." );
            return StatusCode( 500, ModelState );
        }

        return Ok( "Successfully created medicine" );

    }


    [HttpPut("{medicineId}")]
    public async Task<IActionResult> UpdateMedicine(int medicineId, int categoryId ,[FromBody] MedicineDTO updatedMedicine)
    {
        if(updatedMedicine == null)
        {
            return BadRequest( ModelState );
        }

        if(medicineId != updatedMedicine.Id )
        {
            return BadRequest( ModelState );
        }

        if (!await _medicineRepository.MedicineExists( medicineId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var medicine = _mapper.Map<Medicine>( updatedMedicine );

        medicine.Category = await _medicineCategoryRepository.GetMedicineCategoryById( categoryId );
        
        if(! await _medicineRepository.UpdateMedicine(categoryId, medicine))
        {
            ModelState.AddModelError( "", "Something went wrong while updating category" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

    [HttpDelete( "{medicineId}" )]
    [ProducesResponseType( 400 )]
    [ProducesResponseType( 204 )]
    [ProducesResponseType( 404 )]
    public async Task<IActionResult> DeleteMedicine(int medicineId)
    {
        if (!await _medicineRepository.MedicineExists( medicineId ))
        {
            return NotFound();
        }

        var medicineToDelete = await _medicineRepository.GetMedicine( medicineId );
        if (medicineToDelete == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        if (!await _medicineRepository.DeleteMedicine( medicineToDelete ))
        {
            ModelState.AddModelError( "", "Something went wrong when deleting medicine." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

}
