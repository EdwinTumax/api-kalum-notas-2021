using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiKalumNotas.DbContexts;
using ApiKalumNotas.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using ApiKalumNotas.Entities;

namespace ApiKalumNotas.Controllers
{
    [ApiController]
    [Route("/kalum-notas/v1/[controller]")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<CuentasController> logger;
        private readonly KalumNotasDBContext kalumNotasDBContext;

        public CuentasController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, ILogger<CuentasController> logger, KalumNotasDBContext kalumNotasDBContext)
        {
            this.kalumNotasDBContext = kalumNotasDBContext;
            this.roleManager = roleManager;
            this.logger = logger;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.userManager = userManager;

        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser() { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            logger.LogDebug($"Realizando la busqueda del rol con el nombre de {credencialesUsuario.Role}");
            var role = await roleManager.FindByNameAsync("ROLE_STUDENT");
            if (role == null)
            {
                logger.LogWarning($"No se encontro el ROL con el nombre de {credencialesUsuario.Role}");
                return NotFound();
            }
            logger.LogDebug($"Realizando el proceso de cración de usuario");
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);
            if (resultado.Succeeded)
            {
                logger.LogDebug($"Realizando la asignación del Role al usuario");
                await userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, role.Name));
                await userManager.AddToRoleAsync(usuario, credencialesUsuario.Role);
                var roles = await userManager.GetRolesAsync(usuario);
                return ConstruirToken(credencialesUsuario, roles);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
                var roles = await userManager.GetRolesAsync(usuario);
                return ConstruirToken(credencialesUsuario, roles);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }
        private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credencialesUsuario, IList<String> roles)
        {
            Alumno alumno = null;
            var claims = new List<Claim>();
            if (roles.Contains("ROLE_STUDENT"))
            {
                alumno =  this.kalumNotasDBContext.Alumnos.FirstOrDefault(a => a.Email == credencialesUsuario.Email);
                claims.Add(new Claim("apellidos",alumno.Apellidos));
                claims.Add(new Claim("nombres",alumno.Nombres));                
                claims.Add(new Claim("carne", alumno.Carne));
                claims.Add(new Claim("username",credencialesUsuario.Email));
            }
            claims.Add(new Claim("email", credencialesUsuario.Email));                            
            foreach (var rol in roles)
            {
                claims.Add(new Claim("roles", rol));
            }
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);
            return new RespuestaAutenticacion() { Token = new JwtSecurityTokenHandler().WriteToken(securityToken), Expiracion = expiracion };
        }
    }
}