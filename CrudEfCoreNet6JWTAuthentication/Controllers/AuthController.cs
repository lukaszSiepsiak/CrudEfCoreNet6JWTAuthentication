using CrudEfCoreNet6JWTAuthentication.Configurations;
using CrudEfCoreNet6JWTAuthentication.Models;
using CrudEfCoreNet6JWTAuthentication.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudEfCoreNet6JWTAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityUser> _roleManager;
    private readonly JwtConfiguration _jwtConfiguration;

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityUser> roleManager, JwtConfiguration jwtConfiguration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtConfiguration = jwtConfiguration;
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
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already exists"
                    }
                });
            }
            
            //create user
            var newUser = new IdentityUser()
            {
                Email = registrationDto.Email,
                UserName = registrationDto.Name,
            };

            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);

            if (isCreated.Succeeded)
            {
                //generate token
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Server error"
                }
            });
        }    
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        
    }
}