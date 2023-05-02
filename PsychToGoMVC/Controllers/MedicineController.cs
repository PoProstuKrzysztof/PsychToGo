﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using System.Text;

namespace PsychToGo.Client.Controllers;

public class MedicineController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    ///
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri( "https://localhost:7291/api/Medicine" )
    };

    public MedicineController()
    {
    }

    public IActionResult Index()
    {
        List<MedicineDTO> medicines;
        HttpResponseMessage response = _client.GetAsync( _client.BaseAddress + "/list" ).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( data );
        }
        else
        {
            medicines = Enumerable.Empty<MedicineDTO>().ToList();
            ModelState.AddModelError( "", $"There are not medicines" );
        }

        return View( medicines );
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