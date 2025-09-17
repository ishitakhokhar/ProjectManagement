using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Models;
using ProjectManagement.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Dependencies
        private readonly ProjectManagementContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ProjectManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        #endregion

        #region Register
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (_context.SecUsers.Any(u => u.Email == request.Email))
                return BadRequest("User already exists.");

            var defaultRole = _context.SecRoles.FirstOrDefault(r => r.RoleName == "User");
            if (defaultRole == null)
                return BadRequest("Default role 'User' not found. Please seed roles.");

            var user = new SecUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password,  // store raw password ⚠️
                RoleId = defaultRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.SecUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }
        #endregion

        #region Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var existingUser = _context.SecUsers
                .FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (existingUser == null)
                return Unauthorized("Invalid email or password.");

            var role = _context.SecRoles.FirstOrDefault(r => r.RoleId == existingUser.RoleId)?.RoleName ?? "User";
            var token = GenerateJwtToken(existingUser, role);

            return Ok(new LoginResponse
            {
                Token = token,
                FullName = existingUser.FullName,
                Role = role,
                UserId = existingUser.UserId
            });
        }
        #endregion

        #region Generate JWT Token
        private string GenerateJwtToken(SecUser user, string roleName)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region Get Profile (Logged-in User)
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.SecUsers.FindAsync(Convert.ToInt32(userId));
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email
            });
        }
        #endregion

        #region Update Profile (Logged-in User)
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.SecUsers.FindAsync(Convert.ToInt32(userId));
            if (user == null) return NotFound();

            user.FullName = request.FullName;
            user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.PasswordHash = request.NewPassword; // store raw password ⚠️
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully!" });
        }
        #endregion

        #region Get All Users (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.SecUsers.ToList();
            return Ok(users);
        }
        #endregion

        #region Delete User (Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.SecUsers.FindAsync(id);
            if (user == null) return NotFound();

            _context.SecUsers.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        #endregion

        [HttpGet("Dropdown")]
        public async Task<ActionResult<List<object>>> GetUserDropdown()
        {
            var users = await _context.SecUsers
                .Select(u => new { u.UserId, u.FullName })
                .ToListAsync();

            return Ok(users);
        }
    }
}
