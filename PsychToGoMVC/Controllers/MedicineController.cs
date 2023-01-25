using Microsoft.AspNetCore.Mvc;

namespace PsychToGoMVC.Controllers;
public class MedicineController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
}
