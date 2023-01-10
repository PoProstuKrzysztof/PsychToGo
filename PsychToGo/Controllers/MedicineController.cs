using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineController : Controller
{
    private readonly IMedicineRepository _medicineRepository;
    private readonly IMapper _mapper;
    public MedicineController(IMedicineRepository medicineRepository, IMapper mapper)
    {
        _medicineRepository = medicineRepository;
        _mapper = mapper;
    }

    [HttpGet]
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

    [HttpGet("{id}")]
    [ProducesResponseType(200,Type = typeof(ICollection<Medicine>))]
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

}
