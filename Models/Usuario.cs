using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swacv_api.Models
{
    [Table("usuario")]
public class Usuario
{
    [Key]
    [Column("idusuario")]
    public int IdUsuario { get; set; }

    [Column("clave")]
    public string Clave { get; set; } = string.Empty;

    [Column("nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Column("apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("identificadorusuario")]
    public string IdentificadorUsuario { get; set; } = string.Empty;
}
}