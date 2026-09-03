using Microsoft.AspNetCore.Mvc;
using auth10.Models;
using auth10.Data;
using auth10.DTO;
using auth10.Records;
using Microsoft.EntityFrameworkCore;

namespace auth10.Controllers;
[ApiController]
[Route("auth")]
public class AuthControllers: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _factory;

    public AuthControllers(IHttpClientFactory factory, AppDbContext context)
    {
        _context=context;
        _factory=factory;
    }

    [HttpPost("reg")]
    public async Task<IActionResult> RegUser(RegDto dto)
    {
        try
        {
            if(string.IsNullOrEmpty(dto.Name)|| string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Invalid username or password");
        }
        if(await _context.User.AnyAsync(u=> (u.Name ?? "").ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("Username already in use");
        }
        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        var response = new RegResp
        {
            id = user.Id,
            Name = user.Name
        };
        return Ok(response);
        }catch(Exception ex)
        {
            return StatusCode(503,"Service not available try again later");
        }
        

    }
    [HttpPost("log")]
    public async Task<IActionResult> LogUser(LogDto dto)
    {
        if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("please add username and password");

        }
        try
        {
            var user = await _context.User.FirstOrDefaultAsync(u=>(u.Name ?? "").ToLower() == dto.Name.ToLower());
            if(user ==null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword))
            {
                return Unauthorized("Invalid username or Password");
            }
            var response = new LogResp
            {
                Id=user.Id,
                Name = user.Name
            };
            return Ok(response);
            
        }catch(Exception ex)
        {
            return StatusCode(503,"service not available please try again later");
        }

    }
    [HttpGet("puzzle")]
    public async Task<IActionResult> GetPuzzle()
    {
        try
        {
             var client = _factory.CreateClient();
             var url = "https://lichess.org/api/puzzle/daily";

             var response = await client.GetFromJsonAsync<DailyPuzzle>(url);
             if(response?.Puzzle?.Solution== null || response?.Puzzle?.Fen == null)
            {
                return StatusCode(503,"Service not available please try again later");
            }
            return Ok(response);
 
        }catch(Exception ex)
        {
            return StatusCode(503,"Service not available, please try again later");
        }
    }

}