using Microsoft.AspNetCore.Mvc;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.ViewComponents;

public class AdminViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}