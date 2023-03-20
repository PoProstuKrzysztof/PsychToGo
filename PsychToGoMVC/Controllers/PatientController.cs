using Azure;
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
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private readonly IPatientService _patientService;

    Uri baseAdress = new Uri( "https://localhost:7291/api/Patient" );
    HttpClient client = new HttpClient();

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }
 
    public IActionResult Index()
    {
        List<PatientViewModel> patients = new List<PatientViewModel>();
        HttpResponseMessage response = client.GetAsync( client.BaseAddress + "/patients" ).Result;
        if (response.IsSuccessStatusCode)
        {

            string data = response.Content.ReadAsStringAsync().Result;
            patients = JsonConvert.DeserializeObject<List<PatientViewModel>>( data );
        }
        else
        {
            ModelState.AddModelError( "", $"There are not patients" );
            patients = Enumerable.Empty<PatientViewModel>().ToList();
        }

        return View( patients );
    }

    
    [HttpGet]
    public async Task<IActionResult> CreatePatientMVC()
    {
        PatientViewModel? newPatient = new PatientViewModel();

        //Populating patientviewmodel with models
        newPatient.Psychologists = await _patientService.PsychologistsList();
        newPatient.Psychiatrists = await _patientService.PsychiatristsList();
        newPatient.Medicines = await _patientService.MedicinesList();
        newPatient.MedicinesId = new List<int>();
        return View( newPatient );
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatientMVC(PatientViewModel pvm)
    {

        Patient? newPatient = await _patientService.CreatePatientInstance( pvm );

        string data = JsonConvert.SerializeObject( newPatient );


        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );

        //Check if new patients should be created without psychiatrist and medicines
        if (pvm.PsychiatristId == null)
        {
            //if not psychiatrist is assigned 
            HttpResponseMessage responseNoPsychiatrist = client.
                 PostAsync( client.BaseAddress + $"/createNOPSYCH?psychologistId={pvm.PsychologistId}", content ).Result;
            if (responseNoPsychiatrist.IsSuccessStatusCode)
            {
                return RedirectToAction( "Index" );
            }

        }
        else
        {
            //if psychiatrist is assigned
            HttpResponseMessage response = client.
                PostAsync( client.BaseAddress + $"/create?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicinesId.First()}", content ).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction( "Index" );
            }

        }
       
        ModelState.AddModelError( "", $"An error occurred when creating patient" );
        return RedirectToAction( "CreatePatientMVC" );
    }


    
    [HttpGet]
    public IActionResult DeletePatient([FromRoute] int id)
    {

        HttpResponseMessage response = client.DeleteAsync( client.BaseAddress + $"/{id}" ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        return BadRequest();
    }

   
    [HttpGet]
    public async Task<IActionResult> EditPatient([FromRoute] int id)
    {

        PatientViewModel editedPatient = await _patientService.CreateParsedPatientInstance( id );

        if (editedPatient == null)
        {
            return RedirectToAction( "Index" );
        }

        return View( editedPatient );
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPatient(PatientViewModel pvm)
    {

        Patient? updatedPatient = await _patientService.CreatePatientInstance( pvm );

        string data = JsonConvert.SerializeObject( updatedPatient );


        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.
            PutAsync( client.BaseAddress + $"/{pvm.Id}?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicinesId}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred when editing patient" );
        return View( pvm );
    }

    
    [HttpGet]
    public async Task<IActionResult> AssignPsychiatristMVC([FromRoute] int id)
    {
        PatientViewModel? patient = await _patientService.CreateParsedPatientInstance( id );
        if (patient == null)
        {
            ModelState.AddModelError( "", "Error has occured" );
            return RedirectToAction( "Index" );
        }

        patient.Psychiatrists = await _patientService.PsychiatristsList();
        return View( patient );
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPsychiatristMVC([FromBody] PatientViewModel patient)
    {
        Patient? assignedPatient = await _patientService.CreatePatientInstance( patient );

        string data = JsonConvert.SerializeObject( assignedPatient );


        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.
            PutAsync( client.BaseAddress + $"/AssignPsychiatrist?{patient.Id}&psychiatristId={patient.PsychiatristId}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "GetPsychologistPatients", "Psychologist" );
        }
        ModelState.AddModelError( "", $"An error occurred when assignit psychiatrist to patient" );
        return View( patient );
    }
}
