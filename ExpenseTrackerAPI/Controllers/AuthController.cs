using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;

        public AuthController(AppDbContext appDbContext, IConfiguration configuration, IPasswordService passwordService)
        {
            this._db = appDbContext;
            this._configuration = configuration;
            this._passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.UserName))
                return BadRequest("Username is required");

            if (string.IsNullOrWhiteSpace(registerDto.Password))
                return BadRequest("Password is required");

            if (await _db.Users.AnyAsync(u => u.UserName == registerDto.UserName))
                return BadRequest("Username already exists");

            User newUser = new() 
            { 
                UserName = registerDto.UserName, 
                Password = _passwordService.HashPassword(registerDto.Password)
            };

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            return StatusCode(201, new RegisterResponseDto { UserName = newUser.UserName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.UserName))
                return BadRequest("Username is required");

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest("Password is required");

            var user = await _db.Users.Where(u => u.UserName == loginDto.UserName).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            if (!_passwordService.VerifyPassword(loginDto.Password, user.Password))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(1),
            };

            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
