using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.Models;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatient([FromForm] int medicineId,PatientViewModel pvm)
    {
        Patient newPatient = new Patient()
        {
            Name = pvm.Name,
            LastName = pvm.LastName,
            Email = pvm.Email,
            Address = pvm.Address,
            DateOfBirth = pvm.DateOfBirth,
            Phone = pvm.Phone,
            PsychiatristId = pvm.PsychiatristId,
            PsychologistId = pvm.PsychologistId,
            
        };

        string data = JsonConvert.SerializeObject( newPatient );
       
        
        StringContent content = new StringContent(data, Encoding.UTF8,"application/json" );
        HttpResponseMessage response = client.
            PostAsync( client.BaseAddress + $"/create?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicineId}", content ).Result;
        
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( pvm);
    }



    [HttpDelete]
    public async Task<IActionResult> DeletePatient([FromRoute]int patientId)
    {
       
        HttpResponseMessage response =  client.DeleteAsync( client.BaseAddress + "patientId" ).Result;
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        return View(response.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> EditPatient([FromRoute] int patientId)
    {
        PatientViewModel patient = await client.GetFromJsonAsync<PatientViewModel>( $"/{patientId}" );

        return View(patient);
    }
}
