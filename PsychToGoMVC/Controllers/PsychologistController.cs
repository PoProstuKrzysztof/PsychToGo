using Microsoft.AspNetCore.Mvc;

namespace PsychToGoMVC.Controllers;
public class PsychologistController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
