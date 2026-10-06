namespace swacv_api.DTOs
{
    public class PlanificacionLiderDto
    {
        public int IdVacacion { get; set; }

        public int IdEmpleado { get; set; }

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int Dias { get; set; }

        public string? Comentarios { get; set; }

        public string Estatus { get; set; } = string.Empty;
    }
}