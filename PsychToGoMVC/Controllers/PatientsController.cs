using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGoMVC.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace PsychToGoMVC.Controllers;
public class PatientsController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api" );
    HttpClient client = new HttpClient();

    public PatientsController()
    {
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

    public IActionResult Index()
    {
        List<PatientViewModel> patients = new List<PatientViewModel>();
        HttpResponseMessage resposne = client.GetAsync( client.BaseAddress + "/Patient/patients" ).Result;
        if(resposne.IsSuccessStatusCode)
        {
            string data = resposne.Content.ReadAsStringAsync().Result;
            patients =  JsonConvert.DeserializeObject<List<PatientViewModel>>(data);
        }

        return View(patients);
    }

    [HttpGet]
    public IActionResult CreatePatient()
    {
        return View();
    }


    [HttpPost]
    public IActionResult CreatePatient(PatientViewModel pvm)
    {
        string data = JsonConvert.SerializeObject(pvm);
        StringContent content = new StringContent(data, Encoding.UTF8,"application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/Patient/create",content ).Result;
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View();
    }
}
