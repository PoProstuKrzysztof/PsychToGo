using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGoMVC.Models;
using PsychToGoMVC.Services.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class PsychiatristController : Controller
{

    Uri baseAdress = new Uri( "https://localhost:7291/api/Psychiatrist" );
    HttpClient client;

    public PsychiatristController()
    {

        client = new HttpClient();
        client.BaseAddress = baseAdress;
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
            psychiatrists = Enumerable.Empty<PsychiatristDTO>().ToList();

        }

        return View( psychiatrists );
    }

    [HttpGet]
    
    public IActionResult CreatePsychiatrist()
    {
        return View();
    }

    [HttpPost]    
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychiatrist(PsychiatristDTO pvm )
    {
        string data = JsonConvert.SerializeObject( pvm );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/create", content ).Result;

        

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View(pvm);
    }


    [HttpGet]
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
    public async Task<IActionResult> EditPsychiatrist([FromRoute] int id)
    {
        PsychiatristDTO psychiatrist = await client.GetFromJsonAsync<PsychiatristDTO>( client.BaseAddress + $"/{id}" );

        if (psychiatrist == null)
        {
            RedirectToAction("Index");
        }

        return View( psychiatrist );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychiatrist(PsychiatristDTO psychiatrist)
    {
        string data = JsonConvert.SerializeObject( psychiatrist );

        StringContent content = new StringContent(data, Encoding.UTF8,"application/json" );
        HttpResponseMessage response =  client.PutAsync( client.BaseAddress + $"/{psychiatrist.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( psychiatrist );
    }
}
