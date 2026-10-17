namespace swacv_api.DTOs
{
    public class ControlRhEmpleadoDto
    {
        public int IdEmpleado { get; set; }

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Departamento { get; set; } = string.Empty;

        public DateTime? AltaIMSS { get; set; }

        public int DiasVacacionesAnio { get; set; }

        public int DiasDisponibles { get; set; }

        public DateTime? VencimientoVacaciones { get; set; }
    }
}