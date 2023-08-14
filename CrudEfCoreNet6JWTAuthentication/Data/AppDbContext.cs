using CrudEfCoreNet6JWTAuthentication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrudEfCoreNet6JWTAuthentication.Data;

public class AppDbContext: IdentityDbContext
{
    public DbSet<Team> Teams { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}