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

    private readonly HttpClient _client = new HttpClient
    {
        BaseAddress = new Uri( "https://localhost:7291/api/Patient" )
    };

    private readonly IHttpContextAccessor _httpContext;

    public PatientController(IPatientService patientService, IHttpContextAccessor httpContext)
    {
        _patientService = patientService;
        _httpContext = httpContext;
    }

    public async Task<IActionResult> Index()
    {
        HttpResponseMessage response = _client.GetAsync( requestUri: _client.BaseAddress + "/patients" ).Result;
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<PatientViewModel>>();
            return View( data ?? new List<PatientViewModel>() );
        }
        else
        {
            ModelState.AddModelError( "", $"There are no patients" );
            return View( Enumerable.Empty<PatientViewModel>() );
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreatePatientMVC()
    {
        var newPatient = new PatientViewModel()
        {
            Psychologists = await _patientService.PsychologistsList(),
            Psychiatrists = await _patientService.PsychiatristsList(),
            Medicines = await _patientService.MedicinesList(),
            MedicinesId = new List<int>()
        };

        return View( newPatient );
    }

    [HttpGet]
    public async Task<IActionResult> PatientProfileInfo()
    {
        Claim? patientAsUser = _httpContext.HttpContext.User?.FindFirst( ClaimTypes.Name );

        if (patientAsUser == null)
        {
            return NotFound();
        }

        //Getting patient, his psychologist and psychiatrist
        List<PatientDTO> patients = await _client.GetFromJsonAsync<List<PatientDTO>>( _client.BaseAddress + "/patients" );

        int patientId = patients
                .Where( x => x.Email.ToLower() == patientAsUser.Value.ToLower() )
            .Select( x => x.Id )
            .FirstOrDefault();

        //Finding patient psychologist and psychiatrist
        PsychologistDTO patientPsychologist = await _client.GetFromJsonAsync<PsychologistDTO>( _client.BaseAddress + $"/{patientId}/psychologist" );
        PsychiatristDTO patientPsychiatrist = await _client.GetFromJsonAsync<PsychiatristDTO>( _client.BaseAddress + $"/{patientId}/psychiatrist" );

        PatientViewModel patientParsedToPatientViewModel = await _patientService.CreateParsedPatientInstance( patientId );
        patientParsedToPatientViewModel.Psychiatrists = new List<PsychiatristDTO>() { patientPsychiatrist };
        patientParsedToPatientViewModel.Psychologists = new List<PsychologistDTO>() { patientPsychologist };

        HttpResponseMessage medicines = await _client.GetAsync( _client.BaseAddress + $"/{patientId}/medicines" );
        string medicinesData = medicines.Content.ReadAsStringAsync().Result;
        patientParsedToPatientViewModel.Medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( medicinesData );

        return View( patientParsedToPatientViewModel );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatientMVC(PatientViewModel pvm)
    {
        Patient? newPatient = await _patientService.CreatePatientInstance( pvm );
        string data = JsonConvert.SerializeObject( newPatient );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );

        HttpResponseMessage response;

        //Check if new patients should be created without psychiatrist and medicines
        if (pvm.PsychiatristId == null)
        {
            //if not psychiatrist is assigned
            response = _client.
                  PostAsync( _client.BaseAddress + $"/createNOPSYCH?psychologistId={pvm.PsychologistId}", content ).Result;
        }
        else
        {
            //if psychiatrist is assigned
            response = _client.
                PostAsync( _client.BaseAddress + $"/create?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}" +
                $"&medicineId={pvm.MedicinesId.First()}", content ).Result;
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }

        ModelState.AddModelError( "", $"An error occurred while creating patient" );
        return RedirectToAction( "CreatePatientMVC" );
    }

    [HttpGet]
    public IActionResult DeletePatient([FromRoute] int id)
    {
        HttpResponseMessage response = _client.DeleteAsync( _client.BaseAddress + $"/{id}" ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", "An error occured while deleting patient" );
        return BadRequest( ModelState );
    }

    [HttpGet]
    public async Task<IActionResult> EditPatient([FromRoute] int id)
    {
        PatientViewModel editedPatient = await _patientService.CreateParsedPatientInstance( id );

        if (editedPatient == null)
        {
            return RedirectToAction( "Index" );
        }

        try
        {
            //populate patient medicine list with medicines

            HttpResponseMessage medicines = await _client.GetAsync( _client.BaseAddress + $"/{id}/medicines" );
            string medicinesContent = medicines.Content.ReadAsStringAsync().Result;
            editedPatient.Medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( medicinesContent );

            //Finding patient psychologist and psychiatrist
            PsychologistDTO? patientPsychologist = await _client.GetFromJsonAsync<PsychologistDTO>( _client.BaseAddress + $"/{id}/psychologist" );
            PsychiatristDTO? patientPsychiatrist = await _client.GetFromJsonAsync<PsychiatristDTO>( _client.BaseAddress + $"/{id}/psychiatrist" );

            editedPatient.Psychiatrists = new List<PsychiatristDTO>() { patientPsychiatrist };
            editedPatient.Psychologists = new List<PsychologistDTO>() { patientPsychologist };
        }
        catch (Exception)
        {
            ModelState.AddModelError( "", "An error occured with api parsing" );
            return BadRequest( ModelState );
        }

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
            response = _client.
                 PutAsync( _client.BaseAddress + $"/UpdateNoPsychiatrist/{pvm.Id}?psychologistId={pvm.PsychologistId}", content ).Result;
        }
        else
        {
            response = _client.
                PutAsync( _client.BaseAddress + $"/{pvm.Id}?psychologistId={pvm.PsychologistId}&psychiatristId={pvm.PsychiatristId}" +
                $"&medicineId={pvm.MedicinesId}", content ).Result;
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        ModelState.AddModelError( "", $"An error occurred while editing patient" );
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
        HttpResponseMessage response = _client.
            PutAsync( _client.BaseAddress + $"/AssignPsychiatrist?patientId={assignedPatient.Id}" +
            $"&psychiatristId={assignedPatient.PsychiatristId}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "GetPsychologistPatients", "Psychologist" );
        }
        ModelState.AddModelError( "", $"An error occurred when assigning psychiatrist to patient" );
        return View( patient );
    }

    [HttpGet]
    public async Task<IActionResult> AssignMedicineMVC([FromRoute] int id)
    {
        PatientViewModel? patient = await _patientService.CreateParsedPatientInstance( id );
        if (patient == null)
        {
            ModelState.AddModelError( "", "Error has occured" );
            return RedirectToAction( "Index" );
        }

        patient.Medicines = await _patientService.MedicinesList();
        return View( patient );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignMedicineMVC(PatientViewModel patient)
    {
        Patient? assignedPatient = await _patientService.CreatePatientInstance( patient );

        string data = JsonConvert.SerializeObject( assignedPatient );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = _client.
            PutAsync( _client.BaseAddress + $"/AssignMedicine?patientId={assignedPatient.Id}" +
            $"&medicineId={patient.MedicinesId.FirstOrDefault()}", content ).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "GetPsychiatristPatients", "Psychiatrist" );
        }
        ModelState.AddModelError( "", $"An error occurred when assigning medicine to patient" );
        return View( patient );
    }
}