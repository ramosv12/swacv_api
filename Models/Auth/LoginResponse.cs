namespace swacv_api.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }

        public UsuarioLoginResponse Usuario { get; set; } = new();
    }

    public class UsuarioLoginResponse
    {
        public int IdUsuario { get; set; }

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string IdentificadorUsuario { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
    }
}