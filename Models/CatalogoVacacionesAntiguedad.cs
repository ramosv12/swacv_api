using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swacv_api.Models
{
    [Table("catalogo_vacaciones_antiguedad")]
    public class CatalogoVacacionesAntiguedad
    {
        [Key]
        [Column("idvacacionesantiguedad")]
        public int IdVacacionesAntiguedad { get; set; }

        [Column("aniosminimos")]
        public int AniosMinimos { get; set; }

        [Column("aniosmaximos")]
        public int AniosMaximos { get; set; }

        [Column("diasvacaciones")]
        public int DiasVacaciones { get; set; }
    }
}