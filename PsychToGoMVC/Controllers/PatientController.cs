using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGoMVC.Models;
using PsychToGoMVC.Services.Interfaces;
using System.Text;
using System.Text.Json.Nodes;

namespace PsychToGoMVC.Controllers;
public class PatientController : Controller
{
    private readonly IPatientService _patientService;

    Uri baseAdress = new Uri( "https://localhost:7291/api/Patient" );
    HttpClient client = new HttpClient();

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }

    [Authorize(Roles = "admin")]
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
            
        }

        return View(patients);
    }

    [HttpGet]
    
    public async Task<IActionResult> CreatePatientMVC()
    {
        var newPatient = new PatientViewModel();
        newPatient.Psychologists = await _patientService.PsychologistsList();
        newPatient.Psychiatrists = await _patientService.PsychiatristsList();
        newPatient.MedicinesId = await _patientService.MedicinesList();
        newPatient.MedicinesId = new List<int>();
        return View(newPatient);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePatientMVC([FromForm] int medicineId,PatientViewModel pvm)
    {

       Patient? newPatient = _patientService.CreatePatientInstance(pvm);

        string data = JsonConvert.SerializeObject( newPatient );
       
        
        StringContent content = new StringContent(data, Encoding.UTF8,"application/json" );
        HttpResponseMessage response = client.
            PostAsync( client.BaseAddress + $"/create?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicineId}", content ).Result;
        
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return RedirectToAction( "CreatePatientMVC" );
    }



    [HttpGet]
    public IActionResult DeletePatient([FromRoute]int id)
    {
       
        HttpResponseMessage response =  client.DeleteAsync( client.BaseAddress + $"/{id}" ).Result;
        if(response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> EditPatient([FromRoute] int id)
    {

        PatientViewModel editedPatient = await _patientService.CreateParsedPatientInstance( id );

        if(editedPatient == null)
        {
           return RedirectToAction( "Index" );
        }
       
        return View( editedPatient );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditPatient(PatientViewModel pvm)
    {
    
        Patient? updatedPatient = _patientService.CreatePatientInstance( pvm );
       
        string data = JsonConvert.SerializeObject( updatedPatient );


        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response =  client.
            PutAsync( client.BaseAddress + $"/{pvm.Id}?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicineId}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( pvm );
    }
}
