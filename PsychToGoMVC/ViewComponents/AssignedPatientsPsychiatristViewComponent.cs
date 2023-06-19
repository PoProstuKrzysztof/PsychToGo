using Microsoft.AspNetCore.Mvc;

namespace PsychToGo.Client.ViewComponents;

public class AssignedPatientsPsychiatristViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}