using System.ComponentModel.DataAnnotations;

namespace swacv_api.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string CaptchaToken { get; set; } = string.Empty;
    }
}