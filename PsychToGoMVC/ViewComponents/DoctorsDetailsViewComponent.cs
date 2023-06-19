using Microsoft.AspNetCore.Mvc;

namespace PsychToGo.Client.ViewComponents;

public class DoctorsDetailsViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}