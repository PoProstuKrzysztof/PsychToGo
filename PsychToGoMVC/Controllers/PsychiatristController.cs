using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Enums;
using PsychToGo.Client.Services.Interfaces;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.Client.Controllers;

public class PsychiatristController : Controller
{
    private readonly IPsychiatristService _service;

    /// <summary>
    /// Base address to connect with api
    /// </summary>

    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:7291/api/Psychiatrist")
    };

    private readonly IHttpContextAccessor _accessor;

    public PsychiatristController(IHttpContextAccessor accessor, IPsychiatristService service)
    {
        _service = service;
        _accessor = accessor;
    }

    public async Task<IActionResult> Index(string searchBy, string? searchString,
        string sortBy = nameof(PsychiatristDTO.Name),
        SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PsychiatristDTO.Name),"Name" },
            {nameof(PsychiatristDTO.Email),"Email" },
            {nameof(PsychiatristDTO.Address),"Address" },
            {nameof(PsychiatristDTO.DateOfBirth),"Date of Birth" },
            {nameof(PsychiatristDTO.LastName),"Last Name" },
        };

        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/list").Result;
        if (response.IsSuccessStatusCode)
        {
            //Searching
            var data = await response.Content.ReadFromJsonAsync<List<PsychiatristDTO>>();
            data = await _service.GetFilteredPsychiatrists(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sorting
            var sortedPsychiatrists = _service.GetSortedPsychiatrists(data, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            ViewBag.SortData = "psychiatrist";

            return View(sortedPsychiatrists ?? new List<PsychiatristDTO>());
        }
        else
        {
            ModelState.AddModelError("", $"There are no psychiatrists ");
            return View(Enumerable.Empty<PsychiatristDTO>().ToList());
        }
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult CreatePsychiatristMVC()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePsychiatristMVC(PsychiatristDTO pvm)
    {
        if (!ModelState.IsValid) return View();

        string data = JsonConvert.SerializeObject(pvm);
        StringContent content = new(data, Encoding.UTF8, "application/json");
        HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/create", content).Result;

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        ModelState.AddModelError("", $"An error occurred when creating psychiatrist");
        return View(pvm);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult DeletePsychiatrist([FromRoute] int id)
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
    public async Task<IActionResult> EditPsychiatrist([FromRoute] int id)
    {
        PsychiatristDTO psychiatrist = await _client.GetFromJsonAsync<PsychiatristDTO>(_client.BaseAddress + $"/{id}");

        if (psychiatrist == null)
        {
            return RedirectToAction("Index");
        }

        return View(psychiatrist);
    }

    [HttpPost]
    [Authorize(Roles = "admin, psychiatrist")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPsychiatrist(PsychiatristDTO psychiatrist)
    {
        if (!ModelState.IsValid) return View();

        string data = JsonConvert.SerializeObject(psychiatrist);

        StringContent content = new(data, Encoding.UTF8, "application/json");
        HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + $"/{psychiatrist.Id}", content).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        ModelState.AddModelError("", $"An error occurred when editing psychiatrist");
        return View(psychiatrist);
    }

    [HttpGet]
    [Authorize(Roles = "psychiatrist")]
    public async Task<IActionResult> GetPsychiatristPatients()
    {
        //Getting user e-mail here so It can locate his Id in database and view all his patients

        var context = _accessor.HttpContext;

        if (context == null)
        {
            throw new ArgumentException("Http connection wasn't established.");
        }

        Claim? psychiatristAsuser = context.User?.FindFirst(ClaimTypes.Name);

        if (psychiatristAsuser == null)
        {
            return BadRequest();
        }

        List<PsychiatristDTO>? psychiatrists = await _client.GetFromJsonAsync<List<PsychiatristDTO>>(_client.BaseAddress + "/list");

        int? psychiatristId = psychiatrists
            .Where(x => x.Email.ToLower() == psychiatristAsuser.Value.ToLower())
            .Select(x => x.Id).FirstOrDefault();

        List<Patient>? patients = await _client.GetFromJsonAsync<List<Patient>>(_client.BaseAddress + $"/{psychiatristId}/patients");
        if (patients == null)
        {
            ModelState.AddModelError("", "No patients assigned");
            return View(ModelState);
        }

        return View("PatientList", patients);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PsychiatristDetails(int id)
    {
        PsychiatristDTO? psychiatrist = await _client.GetFromJsonAsync<PsychiatristDTO>(_client.BaseAddress + $"/{id}");

        if (psychiatrist == null)
        {
            return NotFound();
        }

        var psychologistPatients = await _client.GetFromJsonAsync<ICollection<PatientDTO>>(_client.BaseAddress + $"/{id}/patients");
        ViewBag.Patients = psychologistPatients;

        return View(psychiatrist);
    }
}