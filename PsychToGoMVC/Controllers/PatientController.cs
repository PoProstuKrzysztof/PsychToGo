using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGoMVC.Models;
using PsychToGoMVC.Services.Interfaces;
using System.Security.Claims;
using System.Text;

namespace PsychToGoMVC.Controllers;

public class PatientController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private readonly IPatientService _patientService;

    private Uri baseAdress = new Uri( "https://localhost:7291/api/Patient" );
    private HttpClient client = new HttpClient();

    private readonly IHttpContextAccessor _httpContext;

    public PatientController(IPatientService patientService, IHttpContextAccessor httpContext)
    {
        _patientService = patientService;
        client = new HttpClient();
        client.BaseAddress = baseAdress;
        _httpContext = httpContext;
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

    [HttpGet]
    public async Task<IActionResult> PatientProfileInfo()
    {
        var patientAsUser = _httpContext.HttpContext.User?.FindFirst( ClaimTypes.Name );

        if (patientAsUser == null)
        {
            return NotFound();
        }

        //Getting patient, his psychologist and psychiatrist
        List<PatientDTO> patients = await client.GetFromJsonAsync<List<PatientDTO>>( client.BaseAddress + "/patients" );

        int patientId = patients.Where( x => x.Email.ToLower() == patientAsUser.Value.ToLower() )
            .Select( x => x.Id ).FirstOrDefault();
              

        PsychologistDTO patientPsychologist = await client.GetFromJsonAsync<PsychologistDTO>( client.BaseAddress + $"/{patientId}/psychologist" );
        PsychiatristDTO patientPsychiatrist = await client.GetFromJsonAsync<PsychiatristDTO>( client.BaseAddress + $"/{patientId}/psychiatrist" );

        PatientViewModel patientParsedToPatientViewModel = await _patientService.CreateParsedPatientInstance( patientId );
        patientParsedToPatientViewModel.Psychiatrists = (ICollection<PsychiatristDTO>)patientPsychiatrist;
        patientParsedToPatientViewModel.Psychologists = ( ICollection<PsychologistDTO> )patientPsychologist;

        var medicines = await client.GetAsync( client.BaseAddress + $"/{patientId}/medicines" );
        var data = medicines.Content.ReadAsStringAsync().Result;
        patientParsedToPatientViewModel.Medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( medicines.Content.ReadAsStringAsync().Result );

        return View( patientParsedToPatientViewModel );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatientMVC(PatientViewModel pvm)
    {
        Patient? newPatient = await _patientService.CreatePatientInstance( pvm );

        string data = JsonConvert.SerializeObject( newPatient );

        HttpResponseMessage response;

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );

        //Check if new patients should be created without psychiatrist and medicines
        if (pvm.PsychiatristId == null)
        {
            //if not psychiatrist is assigned
            response = client.
                  PostAsync( client.BaseAddress + $"/createNOPSYCH?psychologistId={pvm.PsychologistId}", content ).Result;
        }
        else
        {
            //if psychiatrist is assigned
            response = client.
                PostAsync( client.BaseAddress + $"/create?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicinesId.First()}", content ).Result;
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
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

        //populate patient medicine list with medicines
        var medicines = await client.GetAsync( client.BaseAddress + $"/{id}/medicines" );
        var data = medicines.Content.ReadAsStringAsync().Result;
        editedPatient.Medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( medicines.Content.ReadAsStringAsync().Result );
        return View( editedPatient );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPatient(PatientViewModel pvm)
    {
        Patient? updatedPatient = await _patientService.CreatePatientInstance( pvm );

        string data = JsonConvert.SerializeObject( updatedPatient );
        HttpResponseMessage response;

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        if (pvm.PsychiatristId == null)
        {
            response = client.
                 PutAsync( client.BaseAddress + $"/UpdateNoPsychiatrist/{pvm.Id}?psychologistId={pvm.PsychologistId}", content ).Result;
        }
        else
        {
            response = client.
                PutAsync( client.BaseAddress + $"/{pvm.Id}?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}&medicineId={pvm.MedicinesId}", content ).Result;
        }

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
    public async Task<IActionResult> AssignPsychiatristMVC(PatientViewModel patient)
    {
        Patient? assignedPatient = await _patientService.CreatePatientInstance( patient );

        string data = JsonConvert.SerializeObject( assignedPatient );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.
            PutAsync( client.BaseAddress + $"/AssignPsychiatrist?patientId={assignedPatient.Id}&psychiatristId={assignedPatient.PsychiatristId}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "GetPsychologistPatients", "Psychologist" );
        }
        ModelState.AddModelError( "", $"An error occurred when assignit psychiatrist to patient" );
        return View( patient );
    }
}