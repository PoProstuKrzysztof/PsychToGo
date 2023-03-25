using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class MedicineCategoryController : Controller
{
    private readonly IMedicineCategoryRepository _medicineCategoryRepository;
    private readonly IMapper _mapper;

    public MedicineCategoryController(IMedicineCategoryRepository medicineCategoryRepository, IMapper mapper)
    {
        _medicineCategoryRepository = medicineCategoryRepository;
        _mapper = mapper;
    }

    [HttpGet( "list" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<MedicineCategory> ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> GetMedicinesCategories()
    {
        var medicinesCategories = await _medicineCategoryRepository.GetMedicinesCategories();
        if (medicinesCategories == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        return Ok( _mapper.Map<List<MedicineCategoryDTO>>( medicinesCategories ) );
    }

    [HttpGet( "medicines/{categoryName}" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( ICollection<Medicine> ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetMedicinesByCategory(string categoryName)
    {
        if (!await _medicineCategoryRepository.MedicinneCategoryExist( categoryName ))
        {
            return NotFound();
        }

        var medicines = await _medicineCategoryRepository.GetMedicineByCategory( categoryName );
        if (medicines == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        return Ok( _mapper.Map<List<MedicineDTO>>( medicines ) );
    }

    [HttpGet( "{categoryName}" )]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( MedicineCategory ) )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> GetMedicineCategory(string categoryName)
    {
        if (!await _medicineCategoryRepository.MedicinneCategoryExist( categoryName ))
        {
            return NotFound();
        }

        var medicineCategory = await _medicineCategoryRepository.GetMedicineCategory( categoryName );
        if (medicineCategory == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        return Ok( _mapper.Map<MedicineCategoryDTO>( medicineCategory ) );
    }

    [HttpPost( "create" )]
    [ProducesResponseType( StatusCodes.Status201Created )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    public async Task<IActionResult> CreateCategory([FromBody] MedicineCategoryDTO newCategory)
    {
        if (newCategory == null)
        {
            return BadRequest();
        }

        if (await _medicineCategoryRepository.CheckDuplicate( newCategory ))
        {
            ModelState.AddModelError( "Error", "Category already exists." );
            return StatusCode( 422, ModelState );
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        var categoryMap = _mapper.Map<MedicineCategory>( newCategory );
        if (!await _medicineCategoryRepository.CreateCategory( categoryMap ))
        {
            ModelState.AddModelError( "Error", "Something went wrong while saving category." );
            return StatusCode( 500, ModelState );
        }

        return Ok( "Successfully created category" );
    }

    [HttpPut( "{medicineCategoryId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> UpdateMedicineCategory(int medicineCategoryId, [FromBody] MedicineCategoryDTO updatedMedicineCategory)

    {
        if (updatedMedicineCategory == null)
        {
            return BadRequest( ModelState );
        }
        if (medicineCategoryId != updatedMedicineCategory.Id)
        {
            return BadRequest( ModelState );
        }

        if (!await _medicineCategoryRepository.MedicineCategoryExistById( medicineCategoryId ))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        var medicineCategory = _mapper.Map<MedicineCategory>( updatedMedicineCategory );

        if (!await _medicineCategoryRepository.UpdateCategory( medicineCategory ))
        {
            ModelState.AddModelError( "Error", "Something went wrong updating medicineCategory" );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }

    [HttpDelete( "{medicineCategoryId}" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<IActionResult> DeleteMedicineCategory(int medicineCategoryId)
    {
        if (!await _medicineCategoryRepository.MedicineCategoryExistById( medicineCategoryId ))
        {
            return NotFound();
        }

        var medicineCategoryToDelete = await _medicineCategoryRepository.GetMedicineCategoryById( medicineCategoryId );
        if (medicineCategoryToDelete == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest( ModelState );
        }

        if (!await _medicineCategoryRepository.DeleteCategory( medicineCategoryToDelete ))
        {
            ModelState.AddModelError( "Error", "Something went wrong when deleting medicine category." );
            return StatusCode( 500, ModelState );
        }

        return NoContent();
    }
}