using System.ComponentModel.DataAnnotations;

namespace CrudEfCoreNet6JWTAuthentication.Models.Dtos;

public class UserLoginDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password{ get; set; }
}