using Microsoft.AspNetCore.Mvc;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineController : Controller
{
    private readonly IMedicineRepository _medicineRepository;
    public MedicineController(IMedicineRepository medicineRepository)
    {
        _medicineRepository = medicineRepository;
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

        return Ok( medicines );
    }
}
