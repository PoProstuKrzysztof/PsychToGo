using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGo.Models.Identity;
using PsychToGoMVC.Models;
using System.Security.Claims;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class PsychologistController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    Uri baseAdress = new Uri( "https://localhost:7291/api/Psychologist" );
    HttpClient client = new HttpClient();
    
    private readonly IHttpContextAccessor _httpContext;

    public PsychologistController(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

  
    public IActionResult Index()
    {
        List<PsychologistDTO> psychologists = new List<PsychologistDTO>();
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
    public IActionResult CreatePsychologistMVC()
    {
        return View();
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychologistMVC(PsychologistDTO pvm)
    {
        string data = JsonConvert.SerializeObject( pvm );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/create", content ).Result;



        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( pvm );
    }

    
    
    [HttpGet]
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
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychologist(PsychologistDTO psychologist)
    {
        string data = JsonConvert.SerializeObject( psychologist );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PutAsync( client.BaseAddress + $"/{psychologist.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when editing psychologists" );
        return View( psychologist );
    }

  
    [HttpGet]
    public async Task<IActionResult> GetPsychologistPatients()
    {
        //Getting user e-mail here so It can locate his Id in database and view all his patients


        var user = _httpContext.HttpContext.User?.FindFirst(ClaimTypes.Name);
        
        
        if(user == null)
        {
            return BadRequest();
        }

        List<PsychologistDTO> psychologists = await client.GetFromJsonAsync<List<PsychologistDTO>>( client.BaseAddress + "/list" );

        var psychologistId = psychologists.Where(x => x.Email.ToLower() == user.Value.ToLower()).Select(x => x.Id).FirstOrDefault();

        List<Patient> patients = await client.GetFromJsonAsync<List<Patient>>( client.BaseAddress + $"/{psychologistId}/patients" );
        if (patients == null)
        {
            ModelState.AddModelError( "", "Error has occured" );
            return View( ModelState );
        }
        
        return View("PatientList", patients );
    }

}


