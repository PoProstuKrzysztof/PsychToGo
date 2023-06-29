using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace PsychToGo.Client.ViewComponents;

public class PsychiatristViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
