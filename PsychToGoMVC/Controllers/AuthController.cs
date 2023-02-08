using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGoMVC.DTO;
using PsychToGoMVC.Services.Interfaces;
using System.Security.Claims;

namespace PsychToGoMVC.Controllers;
public class AuthController : Controller
{
  private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDTO loginObj = new();
        return View( loginObj );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDTO obj)
    {
        string response = await _authService.LoginAsync<HttpResponseMessage>( obj );
        if(response != string.Empty)
        {
           
            LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>( response );

            var identity = new ClaimsIdentity( CookieAuthenticationDefaults.AuthenticationScheme );
            identity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
            identity.AddClaim( new Claim( ClaimTypes.Role, model.User.Role ) );

            var principal = new ClaimsPrincipal( identity );
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal );

            HttpContext.Session.SetString("JWTToken", model.Token);
            return RedirectToAction("Index","Home");
        }

        return View();

        
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDTO obj)
    {
        var register = await _authService.RegisterAsync<HttpResponseMessage>( obj );
        if(register != string.Empty)
        {
            return RedirectToAction("Login");
        }
       return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public async Task <IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.SetString( "JWTToken", "" );
        return RedirectToAction( "Index", "Home" );
    }
}
