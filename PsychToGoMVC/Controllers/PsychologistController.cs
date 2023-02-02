using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGoMVC.Models;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class PsychologistController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Psychologist" );
    HttpClient client;

    public PsychologistController()
    {

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
            psychologists = Enumerable.Empty<PsychologistDTO>().ToList();
        }

        return View( psychologists );
    }

    [HttpGet]
    public IActionResult CreatePsychologist()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychologist(PsychologistDTO pvm)
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
            RedirectToAction( "Index" );
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
        return View( psychologist );
    }
}


