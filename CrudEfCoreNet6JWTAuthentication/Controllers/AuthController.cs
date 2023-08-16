using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrudEfCoreNet6JWTAuthentication.Models;
using CrudEfCoreNet6JWTAuthentication.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CrudEfCoreNet6JWTAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        if (ModelState.IsValid)
        {
            //Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);

            if (existingUser == null)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string?>
                    {
                        "Invalid payload"
                    },
                    Result = false,
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginDto.Password);

            if (!isCorrect)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string?>()
                    {
                        "Invalid password"
                    },
                    Result = false,
                });
            }

            var jwtToken = GenerateJwtToken(existingUser);

            return Ok(new AuthResult
            {
                Token = jwtToken,
                Result = true
            });
        }

        return BadRequest(new AuthResult
        {
            Errors = new List<string?>()
            {
                "Invalid payload"
            },
            Result = false
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        //validate incoming registration dto
        if (ModelState.IsValid)
        {
            var userExists = await _userManager.FindByEmailAsync(registrationDto.Email);

            if (userExists != null)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string?>
                    {
                        "Email already exists"
                    }
                });
            }
            
            //create user
            var newUser = new IdentityUser
            {
                Email = registrationDto.Email,
                UserName = registrationDto.Name,
            };

            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);

            if (isCreated.Succeeded)
            {
                //generate token
                var token = GenerateJwtToken(newUser);

                return Ok(new AuthResult
                {
                    Result = true,
                    Token = token
                });
            }

            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string?>
                {
                    "Server error",
                }
            });
        }

        return BadRequest();
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var securityKey = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfiguration:Secret").Value ?? string.Empty);
        
        //Create token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("Id",user.Id),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Email,value: user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString(CultureInfo.CurrentCulture)),
            }),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256)
        };

        var token = jwtTokenHandler.CreateToken((tokenDescriptor));
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;

    }
}