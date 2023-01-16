using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGoMVC.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace PsychToGoMVC.Controllers;
public class PatientsController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Patient" );
    HttpClient client = new HttpClient();

    public PatientsController()
    {
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

    public IActionResult Index()
    {
        List<PatientViewModel> patients = new List<PatientViewModel>();
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/patients" ).Result;
        if(response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            patients =  JsonConvert.DeserializeObject<List<PatientViewModel>>(data);
        }
        else
        {
            patients = Enumerable.Empty<PatientViewModel>().ToList();
            ModelState.AddModelError( "", "Something went wrong, try again later" );
        }

        return View(patients);
    }

    [HttpGet]
    public IActionResult CreatePatient()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> CreatePatient(int psychiatristId, int medicineId , int psychologistId,PatientViewModel pvm)
    {

        pvm.PsychiatristId= psychiatristId;
        pvm.PsychologistId = psychologistId;
        pvm.MedicineId= medicineId;
        string data = JsonConvert.SerializeObject( pvm );
        
        StringContent content = new StringContent(data, Encoding.UTF8,"application/json" );
        HttpResponseMessage response = client.PostAsync( client.BaseAddress + "/create",content ).Result;
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View();
    }
}
