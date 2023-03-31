using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGo.Models;
using System.Security.Claims;
using System.Text;

namespace PsychToGoMVC.Controllers;

public class PsychiatristController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private Uri baseAdress = new Uri( "https://localhost:7291/api/Psychiatrist" );

    private HttpClient client = new HttpClient();

    private readonly IHttpContextAccessor _httpContext;
    public PsychiatristController(IHttpContextAccessor httpContext)
    {
        client = new HttpClient();
        client.BaseAddress = baseAdress;
        _httpContext = httpContext;
    }

    public IActionResult Index()
    {
        List<PsychiatristDTO> psychiatrists = new List<PsychiatristDTO>();
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/list" ).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            psychiatrists = JsonConvert.DeserializeObject<List<PsychiatristDTO>>( data );
        }
        else
        {
            ModelState.AddModelError( "", $"There are no psychiatrists " );
            psychiatrists = Enumerable.Empty<PsychiatristDTO>().ToList();
        }

        return View( psychiatrists );
    }

    [HttpGet]
    public IActionResult CreatePsychiatristMVC()
    {
        return View();
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychiatristMVC(PsychiatristDTO pvm)
    {
        string data = JsonConvert.SerializeObject( pvm );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/create", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when creating psychiatrist" );
        return View( pvm );
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
    public IActionResult DeletePsychiatrist([FromRoute] int id)
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
    public async Task<IActionResult> EditPsychiatrist([FromRoute] int id)
    {
        PsychiatristDTO psychiatrist = await client.GetFromJsonAsync<PsychiatristDTO>( client.BaseAddress + $"/{id}" );

        if (psychiatrist == null)
        {
            return NotFound();
        }

        return View( psychiatrist );
    }

    [HttpPost]
    [Authorize( Roles = "admin, psychiatrist")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychiatrist(PsychiatristDTO psychiatrist)
    {
        string data = JsonConvert.SerializeObject( psychiatrist );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PutAsync( client.BaseAddress + $"/{psychiatrist.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when editing psychiatrist" );
        return View( psychiatrist );
    }

    [HttpGet]
    public async Task<IActionResult> GetPsychiatristPatients()
    {
        //Getting user e-mail here so It can locate his Id in database and view all his patients

        var psychiatristAsuser = _httpContext.HttpContext.User?.FindFirst( ClaimTypes.Name );

        if (psychiatristAsuser == null)
        {
            return BadRequest();
        }

        List<PsychiatristDTO>? psychiatrists = await client.GetFromJsonAsync<List<PsychiatristDTO>>( client.BaseAddress + "/list" );

        int? psychiatristId = psychiatrists
            .Where( x => x.Email.ToLower() == psychiatristAsuser.Value.ToLower() )
            .Select( x => x.Id ).FirstOrDefault();

        List<Patient>? patients = await client.GetFromJsonAsync<List<Patient>>( client.BaseAddress + $"/{psychiatristId}/patients" );
        if (patients == null)
        {
            ModelState.AddModelError( "", "No patients assigned" );
            return View( ModelState );
        }


        return View( "PatientList", patients );

    }
}