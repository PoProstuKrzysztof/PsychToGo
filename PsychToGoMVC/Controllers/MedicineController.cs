using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using System.Text;

namespace PsychToGoMVC.Controllers;
public class MedicineController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Medicine" );
    HttpClient client ;

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
    public IActionResult CreateMedicine()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateMedicine(MedicineDTO mdo)
    {
        string data = JsonConvert.SerializeObject( mdo );
        StringContent content = new StringContent( data, Encoding.UTF8, "application/json" );
        HttpResponseMessage response = client.PostAsync(client.BaseAddress + $"/create?medicineId={mdo.CategoryId}",content).Result;

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
}
