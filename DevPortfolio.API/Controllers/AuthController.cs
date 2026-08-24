using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevPortfolio.API.Data;
using DevPortfolio.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DevPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AdminUser> _passwordHasher;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AdminUser>();
        }


        // =====================================================
        // ADMIN LOGIN
        // POST: api/Auth/login
        // =====================================================

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Username and password are required."
                });
            }


            var admin = _context.AdminUsers
                .FirstOrDefault(x =>
                    x.Username == request.Username);


            if (admin == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }


            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    admin,
                    admin.PasswordHash,
                    request.Password
                );


            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }


            // Generate JWT
            var token = GenerateToken(admin);


            return Ok(new LoginResponse
            {
                Token = token,

                Expiration =
                    DateTime.UtcNow.AddMinutes(
                        GetExpirationMinutes()
                    ),

                Username = admin.Username,

                Role = admin.Role
            });
        }


    



        // =====================================================
        // GENERATE JWT
        // =====================================================

        private string GenerateToken(AdminUser admin)
        {
            var jwtKey =
                _configuration["Jwt:Key"];


            var issuer =
                _configuration["Jwt:Issuer"];


            var audience =
                _configuration["Jwt:Audience"];


            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT Key is not configured."
                );
            }


            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    admin.Username
                ),

                new Claim(
                    ClaimTypes.Role,
                    admin.Role
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    admin.Id.ToString()
                )
            };


            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                );


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );


            var expiration =
                DateTime.UtcNow.AddMinutes(
                    GetExpirationMinutes()
                );


            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }



        // =====================================================
        // JWT EXPIRATION
        // =====================================================

        private int GetExpirationMinutes()
        {
            return int.TryParse(
                _configuration["Jwt:ExpirationMinutes"],
                out var minutes)
                ? minutes
                : 60;
        }
    }
}