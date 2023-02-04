using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class MedicineController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Medicine" );
    HttpClient client = new HttpClient();

    public MedicineController()
    {

        client = new HttpClient();
        client.BaseAddress = baseAdress;
    }
    public IActionResult Index()
    {
        List<MedicineDTO> medicines = new List<MedicineDTO>();
        HttpResponseMessage response =  client.GetAsync( client.BaseAddress + "/list" ).Result;
        if(response.IsSuccessStatusCode )
        {
            string data =  response.Content.ReadAsStringAsync().Result;
            medicines = JsonConvert.DeserializeObject<List<MedicineDTO>>( data );
        }
        else
        {
            medicines = Enumerable.Empty<MedicineDTO>().ToList();
        }

        return View(medicines );
    }


    [HttpGet]
    public IActionResult CreateMedicineMVC()
    {
        
        return View();
    }

    [HttpPost]
    
    public IActionResult CreateMedicineMVC(MedicineDTO mdo)
    {
        string data = JsonConvert.SerializeObject( mdo );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync(client.BaseAddress + $"/create",content).Result;

        if(response.IsSuccessStatusCode )
        {
           return RedirectToAction( "Index" );
        }
        return View(mdo);
    }

    [HttpGet]
    public IActionResult DeleteMedicine([FromRoute]int id)
    {
        HttpResponseMessage response =  client.DeleteAsync( client.BaseAddress + $"/{id}" ).Result;
        if(response.IsSuccessStatusCode)
        {
           return RedirectToAction( "Index" );
        }

        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> EditMedicine([FromRoute] int id)
    {
        MedicineDTO medicine = await client.GetFromJsonAsync<MedicineDTO>(client.BaseAddress + $"/{id}");
        if(medicine == null)
        {
            return NotFound();
        }

        return View( medicine );
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditMedicine(MedicineDTO medicine)
    {
        string data = JsonConvert.SerializeObject( medicine );

        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PutAsync( client.BaseAddress + $"/{medicine.Id}", content ).Result;
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction( "Index" );
        }
        return View( medicine );
    }

}
