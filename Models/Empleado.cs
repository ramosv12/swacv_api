using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swacv_api.Models
{
    [Table("empleado")] 
    public class Empleado
    {
        [Key]
        [Column("idempleado")]
        public int IdEmpleado { get; set; }

        [Column("idusuario")]
        public int IdUsuario { get; set; }

        [Column("idequipo")]
        public int IdEquipo { get; set; }

        [Column("diasvacacionesdisponibles")]
        public int DiasVacacionesDisponibles { get; set; }

        [Column("altaimss")]
        public DateTime? AltaIMSS { get; set; }

        [Column("vencimientodiasvacaciones")]
        public DateTime? VencimientoDiasVacaciones { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdEquipo")]
        public Equipo Equipo { get; set; }
    }
}