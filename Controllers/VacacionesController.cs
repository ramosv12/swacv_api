using Microsoft.AspNetCore.Mvc;
using swacv_api.Data;
using Microsoft.EntityFrameworkCore;
using swacv_api.Models;

namespace swacv_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VacacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetVacaciones(int empleadoId, DateTime start, DateTime end)
        {
            //  Convertir a UTC
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var vacaciones = _context.Vacaciones
                .Where(v => v.IdEmpleado == empleadoId &&
                            v.FechaInicio <= end &&
                            v.FechaFin >= start)
                .Select(v => new
                {
                    id = v.IdVacacion,
                    title = "Vacaciones",
                    start = v.FechaInicio,
                    end = v.FechaFin.AddDays(1),
                    color = v.Estatus == "Calendarizado" ? "#4caf50" :   // verde
                        v.Estatus == "Planificado"   ? "#ff9800" :   // naranja
                        v.Estatus == "Autorizado"    ? "#f44336" :   // rojo
                        v.Estatus == "Tomado"        ? "#2196f3" :   // azul
                                                        "#9e9e9e"    // gris (fallback)
                })
                .ToList();

            return Ok(vacaciones);
        }

        [HttpPost]
        public IActionResult CrearVacacion([FromBody] Vacaciones dto)
        {
            try
            {
                var vacacion = new Vacaciones
                {
                    IdEmpleado = 2, // 🔥 fijo por ahora
                    IdLider = 1,    // 🔥 fijo por ahora
                    FechaInicio = dto.FechaInicio.Date,
                    FechaFin = dto.FechaFin.Date,
                    Comentarios = dto.Comentarios,
                    Estatus = "Calendarizado"
                };

                _context.Vacaciones.Add(vacacion);
                _context.SaveChanges();

                return Ok(new { mensaje = "Vacación creada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear vacación",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}