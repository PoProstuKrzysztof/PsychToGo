using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Services.Interfaces;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.Client.Controllers;

public class PsychologistController : Controller
{
    private readonly IPsychologistService _serivce;

    /// <summary>
    /// Base address to connect with api
    /// </summary>
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:7291/api/Psychologist")
    };

    private readonly IHttpContextAccessor _httpContext;

    public PsychologistController(IHttpContextAccessor httpContext, IPsychologistService service)
    {
        _httpContext = httpContext;
        _serivce = service;
    }

    public async Task<IActionResult> Index(string? searchBy, string? searchString,
        string sortBy = nameof(PsychologistDTO.Name),
        SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        ViewBag.SearchFields = new Dictionary<string, string>
        {
            {nameof(PsychologistDTO.Name),"Name" },
            {nameof(PsychologistDTO.Email),"Email" },
            {nameof(PsychologistDTO.Address),"Address" },
            {nameof(PsychologistDTO.DateOfBirth),"Date of Birth" },
            {nameof(PsychologistDTO.LastName),"Last Name" },
        };

        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        if (response.IsSuccessStatusCode)
        {
            //Searching
            var data = await response.Content.ReadFromJsonAsync<List<PsychologistDTO>>();
            data = await _serivce.GetFilteredPsychologist(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sorting
            var sortedPsychologists = _serivce.GetSortedPsychologist(data, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            ViewBag.SortData = "psychologist";

            return View(sortedPsychologists ?? new List<PsychologistDTO>());
        }
        else
        {
            ModelState.AddModelError("", $"There are not psychologists");
            return View(Enumerable.Empty<PsychologistDTO>().ToList());
        }
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult CreatePsychologistMVC()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychologistMVC(PsychologistDTO pvm)
    {
        if (!ModelState.IsValid) return View();

        string data = JsonConvert.SerializeObject(pvm);
        StringContent content = new(data, Encoding.UTF8, "application/json");
        HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/create", content).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        return View(pvm);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult DeletePsychologist([FromRoute] int id)
    {
        HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + $"/{id}").Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        return BadRequest();
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> EditPsychologist([FromRoute] int id)
    {
        PsychologistDTO psychologist = await _client.GetFromJsonAsync<PsychologistDTO>(_client.BaseAddress + $"/{id}");

        if (psychologist == null)
        {
            return RedirectToAction("Index");
        }

        return View(psychologist);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychologist(PsychologistDTO psychologist)
    {
        if (!ModelState.IsValid) return View();

        string data = JsonConvert.SerializeObject(psychologist);

        StringContent content = new(data, Encoding.UTF8, "application/json");
        HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + $"/{psychologist.Id}", content).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        ModelState.AddModelError("", $"An error occurred when editing psychologists");
        return View(psychologist);
    }

    [HttpGet]
    [Authorize(Roles = "psychologist")]
    public async Task<IActionResult> GetPsychologistPatients()
    {
        //Getting user e-mail here so It can locate his Id in database and view all his patients

        Claim? psychologistAsUser = _httpContext.HttpContext.User?.FindFirst(ClaimTypes.Name);

        if (psychologistAsUser == null)
        {
            return BadRequest();
        }

        List<PsychologistDTO> psychologists = await _client.GetFromJsonAsync<List<PsychologistDTO>>(_client.BaseAddress + "/list");

        int psychologistId = psychologists.Where(x => x.Email.ToLower() == psychologistAsUser.Value.ToLower())
            .Select(x => x.Id).FirstOrDefault();

        List<Patient> patients = await _client.GetFromJsonAsync<List<Patient>>(_client.BaseAddress + $"/{psychologistId}/patients");
        if (patients == null)
        {
            ModelState.AddModelError("", "No patients assigned");
            return View(ModelState);
        }

        //it shows only those patients which doesn't have assigned psychiatrist to them
        return View("PatientList",
            patients.
            Where(x => x.PsychiatristId == null)
            .Select(x => x).ToList());
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PsychologistDetails(int id)
    {
        PsychologistDTO? psychologist = await _client.GetFromJsonAsync<PsychologistDTO>(_client.BaseAddress + $"/{id}");

        if (psychologist == null)
        {
            return NotFound();
        }

        var psychologistPatients = await _client.GetFromJsonAsync<ICollection<PatientDTO>>(_client.BaseAddress + $"/{id}/patients");
        ViewBag.Patients = psychologistPatients;

        return View(psychologist);
    }
}