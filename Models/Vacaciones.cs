using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swacv_api.Models
{
    [Table("vacaciones")]
    public class Vacaciones
    {
        [Key]
        [Column("idvacacion")]
        public int IdVacacion { get; set; }

        [Column("idempleado")]
        public int IdEmpleado { get; set; }

        [Column("idlider")]
        public int IdLider { get; set; }

        [Column("fechainicio", TypeName = "date")]
        public DateTime FechaInicio { get; set; }

        [Column("fechafin", TypeName = "date")]
        public DateTime FechaFin { get; set; }

        [Column("comentarios")]
        public string? Comentarios { get; set; }

        [Column("estatus")]
        public string Estatus { get; set; } = string.Empty;
    }
}