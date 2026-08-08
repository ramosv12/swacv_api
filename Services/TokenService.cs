using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using swacv_api.Models;

namespace swacv_api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, int ExpiresIn) GenerarToken(
            Usuario usuario,
            List<string> roles)
        {
            // Obtener configuración del JWT
            string key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Key.");

            string issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Issuer.");

            string audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Audience.");

            int expiresMinutes = int.TryParse(
                _configuration["Jwt:ExpiresMinutes"],
                out int minutos)
                ? minutos
                : 60;

            // La clave debe tener al menos 32 bytes.
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "La clave JWT debe tener al menos 32 bytes.");
            }

            // Claims principales del usuario
            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    usuario.IdUsuario.ToString()),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    usuario.Email),

                new Claim(
                    ClaimTypes.Name,
                    $"{usuario.Nombres} {usuario.Apellidos}".Trim()),

                new Claim(
                    "identificadorUsuario",
                    usuario.IdentificadorUsuario),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            // Agregar uno o varios roles
            foreach (string rol in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        rol)
                );
            }

            // Crear clave de firma
            var securityKey =
                new SymmetricSecurityKey(keyBytes);

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            // Fecha de expiración
            DateTime expiration =
                DateTime.UtcNow.AddMinutes(expiresMinutes);

            // Crear JWT
            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            // Convertir JWT a texto
            string token =
                new JwtSecurityTokenHandler()
                    .WriteToken(jwtToken);

            // Angular recibirá el tiempo en segundos
            int expiresIn = expiresMinutes * 60;

            return (token, expiresIn);
        }
    }
}