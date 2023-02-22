using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsychToGo.DTO;
using PsychToGo.Models.Identity;
using PsychToGoMVC.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PsychToGoMVC.Controllers;
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {       
        _authService = authService;       
    }

    /// <summary>
    /// Get new login request 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDTO loginObj = new();
        return View( loginObj );
    }

    /// <summary>
    /// Logging
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDTO obj)
    {
        string response = await _authService.LoginAsync<HttpResponseMessage>( obj );
        if (response != string.Empty)
        {

            LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>( response );
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken( model.Token );
            //Reading JWT token with claims
            var identity = new ClaimsIdentity( CookieAuthenticationDefaults.AuthenticationScheme );
            identity.AddClaim( new Claim( ClaimTypes.Name, jwt.Claims.FirstOrDefault( u => u.Type == "unique_name" ).Value ) );
            identity.AddClaim( new Claim( ClaimTypes.Role, jwt.Claims.FirstOrDefault( u => u.Type == "role" ).Value ) );

            var principal = new ClaimsPrincipal( identity );
            await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal );

            HttpContext.Session.SetString( "JWTToken", model.Token );
            return RedirectToAction( "Index", "Home" );
        }

        ModelState.AddModelError( "", $"An error occurred while logging in" );
        return View(obj);


    }

    /// <summary>
    /// Get register page
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    /// <summary>
    /// Register
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDTO obj)
    {
        var register = await _authService.RegisterAsync<HttpResponseMessage>( obj );
        if (register != string.Empty)
        {
            return RedirectToAction( "Index", "Home" );
        }
        return View();
    }

    /// <summary>
    /// Get AccessDenied view
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Logout
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.SetString( "JWTToken", "" );
        return RedirectToAction( "Index", "Home" );
    }
}
