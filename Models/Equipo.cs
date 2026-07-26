using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace swacv_api.Models
{
    [Table("equipo")]
    public class Equipo
    {
        [Key]
        [Column("idequipo")]
        public int IdEquipo { get; set; }

        [Column("nombreequipo")]
        public string NombreEquipo { get; set; }

        [Column("descripcionequipo")]
        public string DescripcionEquipo { get; set; }
    }
}