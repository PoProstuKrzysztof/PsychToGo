﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Models;
using PsychToGo.Client.Services.Interfaces;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.Client.Controllers;

public class PatientController : Controller
{
    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private readonly IPatientService _patientService;

    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri( "https://localhost:7291/api/Patient" )
    };

    private readonly IHttpContextAccessor _httpContext;

    public PatientController(IPatientService patientService, IHttpContextAccessor httpContext)
    {
        _patientService = patientService;
        _httpContext = httpContext;
    }

    /// <summary>
    /// Displaying list of patients, with sorting and searching functionality, default option for sorting is ascending
    /// </summary>
    [Authorize( Roles = "admin" )]
    [ResponseCache( CacheProfileName = "Cache60" )]
    public async Task<IActionResult> Index(string searchBy, string? searchString,
        string sortBy = nameof( PatientViewModel.Name ),
        SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PatientViewModel.Name),"Name" },
            {nameof(PatientViewModel.Email),"Email" },
            {nameof(PatientViewModel.Address),"Address" },
            {nameof(PatientViewModel.DateOfBirth),"Date of Birth" },
            {nameof(PatientViewModel.LastName),"Last Name" },
        };

        HttpResponseMessage response = _client.GetAsync( _client.BaseAddress + "/patients" ).Result;
        if (response.IsSuccessStatusCode)
        {
            //Searching
            var data = await response.Content.ReadFromJsonAsync<List<PatientViewModel>>();
            data = await _patientService.GetFilteredPatients( searchBy, searchString );
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sorting
            var sortedPersons = _patientService.GetSortedPatients( data, sortBy, sortOrder );
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View( sortedPersons ?? new List<PatientViewModel>() );
        }
        else
        {
            ModelState.AddModelError( "", $"There are no patients" );
            return View( Enumerable.Empty<PatientViewModel>() );
        }
    }

    [HttpGet]
    [Authorize( Roles = "admin" )]
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
    [Authorize( Roles = "patient" )]
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

        PsychologistDTO patientPsychologist = await _client.GetFromJsonAsync<PsychologistDTO>(
            _client.BaseAddress + $"/{patientId}/psychologist" );

        HttpResponseMessage psychiatrist = await _client.GetAsync( _client.BaseAddress + $"/{patientId}/psychiatrist" );
        PatientViewModel patientParsedToPatientViewModel = await _patientService.CreateParsedPatientInstance( patientId );
        if (psychiatrist.IsSuccessStatusCode)
        {
            patientParsedToPatientViewModel.Psychiatrists = JsonConvert.
                DeserializeObject<List<PsychiatristDTO>>( psychiatrist.Content.ReadAsStringAsync().Result );
        }

        //PsychiatristDTO? patientPsychiatrist = await _client.GetFromJsonAsync<PsychiatristDTO>(
        //_client.BaseAddress + $"/{patientId}/psychiatrist" );
        patientParsedToPatientViewModel.Psychologists = new List<PsychologistDTO>() { patientPsychologist };

        HttpResponseMessage medicines = await _client.GetAsync( _client.BaseAddress + $"/{patientId}/medicines" );
        string medicinesData = medicines.Content.ReadAsStringAsync().Result;
        patientParsedToPatientViewModel.Medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( medicinesData );

        return View( patientParsedToPatientViewModel );
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatientMVC(PatientViewModel pvm)
    {
        Patient? newPatient = await _patientService.CreatePatientInstance( pvm );
        string data = JsonConvert.SerializeObject( newPatient );
        StringContent content = new( data,
            Encoding.UTF8,
            "application/json" );

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
    [Authorize( Roles = "admin" )]
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
    [Authorize( Roles = "admin" )]
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
            ViewBag.PsychologistFullName = $"{patientPsychologist.Name} {patientPsychologist.LastName}";
            ViewBag.PsychiatristFullName = $"{patientPsychiatrist.Name} {patientPsychiatrist.LastName}";
        }
        catch (Exception)
        {
            ModelState.AddModelError( "", "An error occured with api parsing" );
            return BadRequest( ModelState );
        }

        return View( editedPatient );
    }

    [HttpPost]
    [Authorize( Roles = "admin" )]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPatient(PatientViewModel pvm)
    {
        Patient? updatedPatient = await _patientService.CreatePatientInstance( pvm );

        string data = JsonConvert.SerializeObject( updatedPatient );
        HttpResponseMessage response;

        StringContent content = new( data,
            Encoding.UTF8,
            "application/json" );

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
    [Authorize( Roles = "psychologist" )]
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
    [Authorize( Roles = "psychologist" )]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPsychiatristMVC(PatientViewModel patient)
    {
        Patient? assignedPatient = await _patientService.CreatePatientInstance( patient );

        string data = JsonConvert.SerializeObject( assignedPatient );

        StringContent content = new( data,
            Encoding.UTF8,
            "application/json" );

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
    [Authorize( Roles = "psychiatrist" )]
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
    [Authorize( Roles = "psychiatrist" )]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignMedicineMVC(PatientViewModel patient)
    {
        Patient? assignedPatient = await _patientService.CreatePatientInstance( patient );

        string data = JsonConvert.SerializeObject( assignedPatient );

        StringContent content = new( data, Encoding.UTF8, "application/json" );
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