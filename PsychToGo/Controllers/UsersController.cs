﻿using Microsoft.AspNetCore.Mvc;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;

namespace PsychToGo.API.Controllers;

[Route( "api/UsersAuthorize" )]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost( "login" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        LoginResponseDTO loginResponse = await _repository.Login( loginRequest );
        if (loginResponse.User == null || string.IsNullOrEmpty( loginResponse.Token ))
        {
            return BadRequest( new { message = "Username or password is incorrect" } );
        }
        return Ok( loginResponse );
    }

    [HttpPost( "register" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequest)
    {
        bool userNameUnique = _repository.IsUniqueUser( registrationRequest.UserName );
        if (!userNameUnique)
        {
            return BadRequest( new { message = "Username already exists" } );
        }

        UserDTO user = await _repository.Register( registrationRequest );
        if (user == null)
        {
            return BadRequest( new { message = "Error while registering" } );
        }

        return Ok( "Succesfully register" );
    }
}