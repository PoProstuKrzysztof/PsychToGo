using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Services.Interfaces;
using System.Text;

namespace PsychToGo.Client.Controllers;

[Route( "medicines" )]
public class MedicineController : Controller
{
    private readonly IMedicineService _service;

    /// <summary>
    /// Base address to connect with api
    /// </summary>
    ///
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri( "https://localhost:7291/api/Medicine" )
    };

    public MedicineController(IMedicineService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string searchBy, string? searchString)
    {
        ViewBag.SearchFields = new Dictionary<string, string>
        {
            {nameof(MedicineDTO.Name), "Name"},
            {nameof(MedicineDTO.ExpireDate), "Expire date"},
            {nameof(MedicineDTO.InStock),"In stock" },
            {nameof(MedicineDTO.ProductionDate),"Production date" }
        };

        HttpResponseMessage response = _client.GetAsync( _client.BaseAddress + "/list" ).Result;
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<MedicineDTO>>();
            data = await _service.GetFilteredMedicines( searchBy, searchString );
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            return View( data ?? new List<MedicineDTO>() );
        }
        else
        {
            ModelState.AddModelError( "", $"There are not medicines" );
            return View( Enumerable.Empty<MedicineDTO>().ToList() );
        }
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public IActionResult CreateMedicineMVC()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize( Roles = "admin" )]
    public IActionResult CreateMedicineMVC(MedicineDTO mdo)
    {
        string data = JsonConvert.SerializeObject( mdo );
        StringContent content = new( data,
            Encoding.UTF8,
            "application/json" );

        HttpResponseMessage response = _client.PostAsync( _client.BaseAddress + $"/create", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"Something went wrong when creating medicine." );
        return View( mdo );
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public IActionResult DeleteMedicine([FromRoute] int id)
    {
        HttpResponseMessage response = _client.DeleteAsync( _client.BaseAddress + $"/{id}" ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        return BadRequest();
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public async Task<IActionResult> EditMedicine([FromRoute] int id)
    {
        MedicineDTO medicine = await _client.GetFromJsonAsync<MedicineDTO>( _client.BaseAddress + $"/{id}" );
        if (medicine == null)
        {
            return NotFound();
        }

        return View( medicine );
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public IActionResult EditMedicine(MedicineDTO medicine)
    {
        string data = JsonConvert.SerializeObject( medicine );

        StringContent content = new( data,
            Encoding.UTF8,
            "application/json" );
        HttpResponseMessage response = _client.PutAsync( _client.BaseAddress + $"/{medicine.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when editing medicine" );
        return View( medicine );
    }
}