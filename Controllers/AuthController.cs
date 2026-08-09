using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swacv_api.Data;
using swacv_api.Models.Auth;
using swacv_api.Services;

namespace swacv_api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public AuthController(
            AppDbContext context,
            PasswordService passwordService,
            TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request)
        {
            // --------------------------------------------------
            // CAPTCHA
            // --------------------------------------------------
            // La validación real del CAPTCHA se agregará
            // en el siguiente paso.


            // --------------------------------------------------
            // NORMALIZAR EMAIL
            // --------------------------------------------------

            string email = request.Email
                .Trim()
                .ToLowerInvariant();


            // --------------------------------------------------
            // BUSCAR USUARIO
            // --------------------------------------------------

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Email.ToLower() == email
                );


            // Si el correo no existe
            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }


            // --------------------------------------------------
            // VALIDAR CONTRASEÑA
            // --------------------------------------------------

            bool passwordCorrecta =
                _passwordService.VerificarPassword(
                    usuario,
                    request.Password
                );


            if (!passwordCorrecta)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }


            // --------------------------------------------------
            // DETERMINAR SI ES EMPLEADO
            // --------------------------------------------------

            bool esEmpleado = await _context.Empleados
                .AsNoTracking()
                .AnyAsync(
                    e => e.IdUsuario == usuario.IdUsuario
                );


            // --------------------------------------------------
            // DETERMINAR SI ES LÍDER
            // --------------------------------------------------

            bool esLider = await _context.Lideres
                .AsNoTracking()
                .AnyAsync(
                    l => l.IdUsuario == usuario.IdUsuario
                );


            // --------------------------------------------------
            // CONSTRUIR LISTA DE ROLES
            // --------------------------------------------------

            var roles = new List<string>();

            if (esEmpleado)
            {
                roles.Add("Empleado");
            }

            if (esLider)
            {
                roles.Add("Lider");
            }


            // --------------------------------------------------
            // VALIDAR QUE TENGA AL MENOS UN PERFIL
            // --------------------------------------------------

            if (roles.Count == 0)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        mensaje =
                            "El usuario no tiene un perfil de empleado o líder asociado."
                    }
                );
            }


            // --------------------------------------------------
            // GENERAR JWT
            // --------------------------------------------------

            var resultadoToken =
                _tokenService.GenerarToken(
                    usuario,
                    roles
                );


            // --------------------------------------------------
            // CONSTRUIR RESPUESTA
            // --------------------------------------------------

            var response = new LoginResponse
            {
                Token = resultadoToken.Token,

                ExpiresIn = resultadoToken.ExpiresIn,

                Usuario = new UsuarioLoginResponse
                {
                    IdUsuario = usuario.IdUsuario,

                    Nombres = usuario.Nombres,

                    Apellidos = usuario.Apellidos,

                    Email = usuario.Email,

                    IdentificadorUsuario =
                        usuario.IdentificadorUsuario,

                    Roles = roles
                }
            };


            return Ok(response);
        }
    }
}