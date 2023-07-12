using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntrevistaSoloTalentoBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace EntrevistaSoloTalentoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly EntrevistaSoloTalentoContext _context;

        public UsersController(IConfiguration configuration, EntrevistaSoloTalentoContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        [Route("register")]
        public dynamic Register(Users credentials)
        {
            if (_context.Users == null)
            {
                return BadRequest(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
            }
            

            Users user = new Users();

            try
            {
                string username= credentials.Username;
                string password= credentials.Password;
                bool? rol = credentials.IsManager;
                int id = _context.Users.OrderByDescending(x => x.UserID).Select(x => x.UserID).FirstOrDefault();
                user.UserID = id + 1;
                user.Username = username;
                user.Password = Encrypt.GetSHA256(password);
                user.IsManager = rol;
                
                _context.Users.Add(user);
                _context.SaveChanges();

                // Obtenemos la configuración de Jwt del appsettings.json
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                // Especificamos los claims
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("username", user.Username),
                new Claim("id", user.UserID.ToString()),
                new Claim("admin", rol.ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: signIn
                );
                return Ok(new { error = false, statusCode = 200, message = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message, statusCode = 500 });
            }
        }

        [HttpPost]
        [Route("login")]
        public dynamic Login(Users credentials)
        {
            string username = credentials.Username;
            string password = Encrypt.GetSHA256(credentials.Password);

            Users user = _context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user == null)
            {
                return NotFound(new { error = true, message = "Usuario o contraseña incorrectos, por favor verifique sus credenciales", statusCode = 401 });
            }

            int id = user.UserID;
            bool isAdmin = (bool)user.IsManager;

            // Obtenemos la configuración de Jwt del appsettings.json
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            // Especificamos los claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("username", user.Username),
                new Claim("id", id.ToString()),
                new Claim("admin", isAdmin.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signIn
            );

            return Ok(new { error = false, statusCode = 200, message = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // DELETE: api/Users/5
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}
