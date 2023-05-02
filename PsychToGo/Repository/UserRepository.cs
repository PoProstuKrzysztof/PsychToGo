using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PsychToGo.API.Data;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.API.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private string? secretKey;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext context, IConfiguration configuration, UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        secretKey = configuration.GetValue<string>( "ApiSettings:Secret" );
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public bool IsUniqueUser(string username)
    {
        AppUser? user = _context.ApplicationUsers.FirstOrDefault( x => x.UserName == username );
        if (user == null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Login to application using passowrd and email
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
    {
        AppUser? user = _context.ApplicationUsers
            .FirstOrDefault( x => x.UserName.ToLower() == loginRequest.UserName.ToLower() );

        bool isValidPassword = await _userManager.CheckPasswordAsync( user, loginRequest.Password );
        if (!isValidPassword)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = _mapper.Map<UserDTO>( user )
            };
        }

        if (user == null)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = _mapper.Map<UserDTO>( user )
            };
        }

        //generating JWT token
        IList<string> roles = await _userManager.GetRolesAsync( user );
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes( secretKey );

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity( new Claim[]
            {
                new Claim( ClaimTypes.Name, user.UserName.ToString() ),
                //Adding roles to user, for now its first role that iterator encounters
                new Claim( ClaimTypes.Role, roles.FirstOrDefault() )
            } ),
            Expires = DateTime.UtcNow.AddDays( 14 ),
            SigningCredentials = new( new SymmetricSecurityKey( key ), SecurityAlgorithms.HmacSha256Signature )
        };

        SecurityToken token = tokenHandler.CreateToken( tokenDescriptor );
        LoginResponseDTO loginResponse = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken( token ),
            User = _mapper.Map<UserDTO>( user )
        };

        return loginResponse;
    }

    /// <summary>
    /// Registration request to register into application
    /// </summary>
    /// <param name="registrationRequest"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequest)
    {
        AppUser user = new AppUser()
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.UserName,
            NormalizedEmail = registrationRequest.UserName.ToUpper(),
            Name = registrationRequest.Name,
            LastName = registrationRequest.LastName
        };

        try
        {
            //For development, change role name every time if you want to assign user to specific role
            IdentityResult result = await _userManager.CreateAsync( user, registrationRequest.Password );
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync( user, registrationRequest.Role );

                AppUser? userToReturn = _context.ApplicationUsers.FirstOrDefault( u => u.UserName == registrationRequest.UserName );
                return _mapper.Map<UserDTO>( userToReturn );
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        return new UserDTO();
    }
}