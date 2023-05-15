using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.Client.Controllers;

public class PsychologistController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private readonly HttpClient client = new()
    {
        BaseAddress = new Uri( "https://localhost:7291/api/Psychologist" )
    };

    private readonly IHttpContextAccessor _httpContext;

    public PsychologistController(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public IActionResult Index()
    {
        List<PsychologistDTO> psychologists;
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/list" ).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            psychologists = JsonConvert.DeserializeObject<List<PsychologistDTO>>( data );
        }
        else
        {
            ModelState.AddModelError( "", $"There are not psychologists" );
            psychologists = Enumerable.Empty<PsychologistDTO>().ToList();
        }

        return View( psychologists );
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public IActionResult CreatePsychologistMVC()
    {
        return View();
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychologistMVC(PsychologistDTO pvm)
    {
        string data = JsonConvert.SerializeObject( pvm );
        StringContent content = new( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/create", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( pvm );
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public IActionResult DeletePsychologist([FromRoute] int id)
    {
        HttpResponseMessage response = client.DeleteAsync( client.BaseAddress + $"/{id}" ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        return BadRequest();
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public async Task<IActionResult> EditPsychologist([FromRoute] int id)
    {
        PsychologistDTO psychologist = await client.GetFromJsonAsync<PsychologistDTO>( client.BaseAddress + $"/{id}" );

        if (psychologist == null)
        {
            return RedirectToAction( "Index" );
        }

        return View( psychologist );
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychologist(PsychologistDTO psychologist)
    {
        string data = JsonConvert.SerializeObject( psychologist );

        StringContent content = new( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PutAsync( client.BaseAddress + $"/{psychologist.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when editing psychologists" );
        return View( psychologist );
    }

    [HttpGet]
    [Authorize( Roles = "psychologist" )]
    public async Task<IActionResult> GetPsychologistPatients()
    {
        //Getting user e-mail here so It can locate his Id in database and view all his patients

        Claim? psychologistAsUser = _httpContext.HttpContext.User?.FindFirst( ClaimTypes.Name );

        if (psychologistAsUser == null)
        {
            return BadRequest();
        }

        List<PsychologistDTO> psychologists = await client.GetFromJsonAsync<List<PsychologistDTO>>( client.BaseAddress + "/list" );

        int psychologistId = psychologists.Where( x => x.Email.ToLower() == psychologistAsUser.Value.ToLower() )
            .Select( x => x.Id ).FirstOrDefault();

        List<Patient> patients = await client.GetFromJsonAsync<List<Patient>>( client.BaseAddress + $"/{psychologistId}/patients" );
        if (patients == null)
        {
            ModelState.AddModelError( "", "No patients assigned" );
            return View( ModelState );
        }

        //it shows only those patients which doesn't have assigned psychiatrist to them
        return View( "PatientList",
            patients.
            Where( x => x.PsychiatristId == null )
            .Select( x => x ).ToList() );
    }
}