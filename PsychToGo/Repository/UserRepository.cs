using Microsoft.IdentityModel.Tokens;
using PsychToGo.Data;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PsychToGo.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private string secretKey;

    public UserRepository(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        secretKey = configuration.GetValue<string>( "ApiSettings:Secret" );
    }

    public bool IsUniqueUser(string username)
    {
        var user = _context.LocalUsers.FirstOrDefault(x => x.UserName == username);
        if(user == null)
        {
            return true ;
        }

        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
    {
        var user = _context.LocalUsers.FirstOrDefault( x => x.UserName.ToLower() == loginRequest.UserName.ToLower()
        && x.Password == loginRequest.Password);
        if(user == null)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = user
            };
        }

        //generating JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity( new Claim[]
            {
                new Claim( ClaimTypes.Name, user.Id.ToString() ),
                new Claim( ClaimTypes.Role, user.Role )
            } ),
            Expires = DateTime.UtcNow.AddDays( 14 ),
            SigningCredentials = new( new SymmetricSecurityKey( key ), SecurityAlgorithms.HmacSha256Signature )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor );
        LoginResponseDTO loginResponse = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = user,
        };

        return loginResponse;
    }

    public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequest)
    {
        LocalUser user = new LocalUser()
        {
            UserName= registrationRequest.UserName,
            Name= registrationRequest.Name,
            Password= registrationRequest.Password,
            Role = registrationRequest.Role

        };

        _context.LocalUsers.Add(user);
       await _context.SaveChangesAsync();

        user.Password = "";
        return user;
    }
}
