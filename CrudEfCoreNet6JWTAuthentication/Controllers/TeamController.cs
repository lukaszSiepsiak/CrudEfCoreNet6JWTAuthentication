using CrudEfCoreNet6JWTAuthentication.Data;
using CrudEfCoreNet6JWTAuthentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudEfCoreNet6JWTAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamController: ControllerBase
{
    private static AppDbContext _context;

    public TeamController(AppDbContext context)
    {
        _context = context;
    }
  
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var teams = await _context.Teams.ToListAsync();
        
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == id);

        if (team == null)
            return BadRequest($"There is no team in this ID: {id}");

        return Ok(team);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Team team)
    {
        if (team == null)
            return BadRequest("You cannot specify team");
        
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        return CreatedAtAction("Get", team.Id, team);
    }

    [HttpPut]
    public async Task<IActionResult> Put(int id, string country)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(team => team.Id == id);
        
        if (team == null)
            return BadRequest($"Team with ID:{id} does not exists!");

        if (String.IsNullOrWhiteSpace(country))
            return BadRequest("You must correctly specify country property!");
        
        team.Country = country;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(team => team.Id == id);
        
        if (team == null)
            return BadRequest($"Team with ID:{id} does not exists!");

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}