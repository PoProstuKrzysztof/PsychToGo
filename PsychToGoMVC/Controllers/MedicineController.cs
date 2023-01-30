using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;

namespace PsychToGoMVC.Controllers;
public class MedicineController : Controller
{
    Uri baseAdress = new Uri( "https://localhost:7291/api/Medicine" );
    HttpClient client;

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
}
