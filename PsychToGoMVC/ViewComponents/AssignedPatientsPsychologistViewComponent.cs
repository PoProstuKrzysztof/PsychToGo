using Microsoft.AspNetCore.Mvc;

namespace PsychToGo.Client.ViewComponents;

public class AssignedPatientsPsychologistViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}