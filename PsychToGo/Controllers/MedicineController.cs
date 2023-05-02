using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGo.API.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class MedicineController : Controller
{
    private readonly IMedicineRepository _medicineRepository;
    private readonly IMedicineCategoryRepository _medicineCategoryRepository;
    private readonly IMapper _mapper;

    public MedicineController(IMedicineRepository medicineRepository, IMapper mapper, IMedicineCategoryRepository medicineCategoryRepository)
    {
        _medicineRepository = medicineRepository;
        _medicineCategoryRepository = medicineCategoryRepository;
        _mapper = mapper;
    }

    [HttpGet( "list" )]
    [ResponseCache( CacheProfileName = "Cache60" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<Medicine> ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> GetMedicines()
    {
        ICollection<Medicine> medicines = await _medicineRepository.GetMedicines();
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok( _mapper.Map<List<MedicineDTO>>( medicines ) );
    }

    [HttpGet( "{medicineId}/inStock" )]
    [ResponseCache( CacheProfileName = "Cache60" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetMedicineInStock(int medicineId)
    {
        if (!await _medicineRepository.MedicineExists( medicineId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        int inStock = await _medicineRepository.GetMedicineInStock( medicineId );

        return Ok( inStock );
    }

    [HttpGet( "{medicineId}/ExpireDate" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetMedicineExpireDate(int medicineId)
    {
        if (!await _medicineRepository.MedicineExists( medicineId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        string medicineExpireDate = await _medicineRepository.GetMedicineExpireDate( medicineId );
        if (medicineExpireDate == null)
        {
            return NotFound();
        }

        return Ok( medicineExpireDate );
    }

    [HttpGet( "{id}" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( MedicineDTO ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetMedicineById(int id)
    {
        if (!await _medicineRepository.MedicineExists( id ))
        {
            return NotFound();
        }

        Medicine medicine = await _medicineRepository.GetMedicine( id );
        if (medicine == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        return Ok( _mapper.Map<MedicineDTO>( medicine ) );
    }

    [HttpPost( "create" )]
    [ProducesResponseType( StatusCodes.Status201Created )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> CreateMedicine([FromBody] MedicineDTO newMedicine)
    {
        if (newMedicine == null)
        {
            return NotFound();
        }

        if (await _medicineRepository.CheckDuplicate( newMedicine ))
        {
            ModelState.AddModelError( "", "Medicine already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        Medicine medicine = _mapper.Map<Medicine>( newMedicine );
        medicine.Category = await _medicineCategoryRepository.GetMedicineCategoryById( newMedicine.CategoryId );
        if (!await _medicineRepository.CreateMedicine( newMedicine.CategoryId, medicine ))
        {
            ModelState.AddModelError( "Error", "Something went wrong while saving medicine." );
            return StatusCode( 500, ModelState );
        }

        return Created( "Successfully created medicine", medicine );
    }

    [HttpPut( "{medicineId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> UpdateMedicine(int medicineId, [FromBody] MedicineDTO updatedMedicine)
    {
        if (updatedMedicine == null)
        {
            return NotFound();
        }

        if (medicineId != updatedMedicine.Id)
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

        Medicine medicine = _mapper.Map<Medicine>( updatedMedicine );

        medicine.Category = await _medicineCategoryRepository.GetMedicineCategoryById( updatedMedicine.CategoryId );

        if (!await _medicineRepository.UpdateMedicine( updatedMedicine.CategoryId, medicine ))
        {
            ModelState.AddModelError( "Error", "Something went wrong while updating category" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

    [HttpDelete( "{medicineId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> DeleteMedicine(int medicineId)
    {
        if (!await _medicineRepository.MedicineExists( medicineId ))
        {
            return NotFound();
        }

        Medicine medicineToDelete = await _medicineRepository.GetMedicine( medicineId );
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
            ModelState.AddModelError( "Error", "Something went wrong when deleting medicine." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }
}