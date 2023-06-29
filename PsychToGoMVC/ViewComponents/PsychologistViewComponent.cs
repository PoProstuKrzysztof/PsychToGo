using Microsoft.AspNetCore.Mvc;

namespace PsychToGo.Client.ViewComponents;

public class PsychologistViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}