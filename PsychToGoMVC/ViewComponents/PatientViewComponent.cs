using Microsoft.AspNetCore.Mvc;

namespace PsychToGo.Client.ViewComponents;

public class PatientViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}