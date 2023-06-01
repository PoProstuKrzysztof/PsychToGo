using Microsoft.AspNetCore.Mvc;
using PsychToGo.API.DTO;

namespace PsychToGo.Client.ViewComponents;

public class LoginViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}