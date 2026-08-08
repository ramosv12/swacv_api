using Microsoft.AspNetCore.Identity;
using swacv_api.Models;

namespace swacv_api.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        // Genera el hash que se almacenará en usuario.clave
        public string GenerarHash(Usuario usuario, string password)
        {
            return _passwordHasher.HashPassword(usuario, password);
        }

        // Verifica una contraseña contra el hash almacenado
        public bool VerificarPassword(
            Usuario usuario,
            string passwordIngresado)
        {
            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Clave,
                passwordIngresado
            );

            return resultado == PasswordVerificationResult.Success ||
                   resultado == PasswordVerificationResult.SuccessRehashNeeded;
        }

        // Valida que una contraseña asignada sea fuerte
        public bool EsPasswordFuerte(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 10)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsLower))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }
    }
}