using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swacv_api.Models
{
    [Table("lider")]
    public class Lider
    {
        [Key]
        [Column("idlider")]
        public int IdLider { get; set; }

        [Column("idusuario")]
        public int? IdUsuario { get; set; }

        [Column("idequipo")]
        public int? IdEquipo { get; set; }
    }
}