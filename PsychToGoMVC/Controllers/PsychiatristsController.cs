using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGoMVC.Models;
using System.Collections.Generic;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class PsychiatristsController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Psychiatrist" );
    HttpClient client = new HttpClient();

    public PsychiatristsController()
    {
        
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

    public IActionResult Index()
    {
        List<PsychiatristViewModel> psychiatrists = new List<PsychiatristViewModel>();
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/list" ).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            psychiatrists = JsonConvert.DeserializeObject<List<PsychiatristViewModel>>( data );
        }
        else
        {
            psychiatrists = Enumerable.Empty<PsychiatristViewModel>().ToList();
            ModelState.AddModelError( "", "Something went wrong, try again later" );
        }

        return View( psychiatrists );      
    }

    [HttpGet]
    public IActionResult CreatePsychiatrist()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePsychiatrist(PsychiatristViewModel pvm )
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
}
