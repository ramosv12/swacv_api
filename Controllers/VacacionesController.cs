using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swacv_api.Data;
using swacv_api.Models;
using swacv_api.DTOs;

namespace swacv_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VacacionesController(AppDbContext context)
        {
            _context = context;
        }
        private static int CalcularDiasLaborales(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            // Validar que la fecha final no sea menor que la inicial
            if (fechaFin.Date < fechaInicio.Date)
            {
                return 0;
            }

            int dias = 0;

            for (
                DateTime fecha = fechaInicio.Date;
                fecha <= fechaFin.Date;
                fecha = fecha.AddDays(1)
            )
            {
                if (
                    fecha.DayOfWeek != DayOfWeek.Saturday &&
                    fecha.DayOfWeek != DayOfWeek.Sunday
                )
                {
                    dias++;
                }
            }

            return dias;
        }

        [HttpGet]
        public IActionResult GetVacaciones(
            int empleadoId,
            DateTime start,
            DateTime end)
        {
            // Convertir a UTC
            start = DateTime.SpecifyKind(
                start,
                DateTimeKind.Utc
            );

            end = DateTime.SpecifyKind(
                end,
                DateTimeKind.Utc
            );

            var vacaciones = _context.Vacaciones
                .Where(v =>
                    v.IdEmpleado == empleadoId &&
                    v.FechaInicio <= end &&
                    v.FechaFin >= start
                )
                .Select(v => new
                {
                    id = v.IdVacacion,
                    title = "Vacaciones",
                    start = v.FechaInicio,
                    end = v.FechaFin.AddDays(1),

                    color =
                        v.Estatus == "Calendarizado" ? "#4caf50" :
                        v.Estatus == "Planificado"   ? "#ff9800" :
                        v.Estatus == "Autorizado"    ? "#f44336" :
                        v.Estatus == "Tomado"        ? "#2196f3" :
                                                       "#9e9e9e"
                })
                .ToList();

            return Ok(vacaciones);
        }

        [HttpGet("planificacion-lider/{idLider}")]
        public IActionResult GetPlanificacionLider(int idLider)
        {
            var datos = (
                from vacacion in _context.Vacaciones

                join empleado in _context.Empleados
                    on vacacion.IdEmpleado equals empleado.IdEmpleado

                join usuario in _context.Usuarios
                    on empleado.IdUsuario equals usuario.IdUsuario

                where vacacion.IdLider == idLider

                orderby vacacion.FechaInicio

                select new
                {
                    vacacion.IdVacacion,
                    vacacion.IdEmpleado,

                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Email,

                    vacacion.FechaInicio,
                    vacacion.FechaFin,

                    vacacion.Comentarios,
                    vacacion.Estatus
                }
            ).ToList();

            var resultado = datos
                .Select(x => new PlanificacionLiderDto
                {
                    IdVacacion = x.IdVacacion,

                    IdEmpleado = x.IdEmpleado,

                    Nombres = x.Nombres,

                    Apellidos = x.Apellidos,

                    Correo = x.Email,

                    FechaInicio = x.FechaInicio,

                    FechaFin = x.FechaFin,

                    Dias = CalcularDiasLaborales(
                        x.FechaInicio,
                        x.FechaFin
                    ),

                    Comentarios = x.Comentarios,

                    Estatus = x.Estatus
                })
                .ToList();

            return Ok(resultado);
        }

        [HttpPatch("{idVacacion}/estatus")]
        public IActionResult ActualizarEstatusVacacion(
            int idVacacion,
            [FromBody] ActualizarEstatusVacacionDto dto)
        {
            // Estados permitidos dentro del sistema
            string[] estadosPermitidos =
            {
                "Calendarizado",
                "Planificado",
                "Autorizado",
                "Tomado"
            };

            // Validar que se haya enviado un estado
            if (string.IsNullOrWhiteSpace(dto.Estatus))
            {
                return BadRequest(new
                {
                    mensaje = "El estatus es obligatorio."
                });
            }

            // Buscar el estado permitido ignorando mayúsculas/minúsculas
            var nuevoEstatus = estadosPermitidos
                .FirstOrDefault(
                    estado =>
                        estado.Equals(
                            dto.Estatus.Trim(),
                            StringComparison.OrdinalIgnoreCase
                        )
                );

            // Validar que el estado enviado sea válido
            if (nuevoEstatus == null)
            {
                return BadRequest(new
                {
                    mensaje = "El estatus enviado no es válido."
                });
            }

            // Buscar la vacación
            var vacacion = _context.Vacaciones
                .FirstOrDefault(
                    v => v.IdVacacion == idVacacion
                );

            if (vacacion == null)
            {
                return NotFound(new
                {
                    mensaje = "La vacación no fue encontrada."
                });
            }

            // Una vacación con estatus Tomado
            // ya no puede cambiar de estado
            if (
                vacacion.Estatus.Equals(
                    "Tomado",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Conflict(new
                {
                    mensaje =
                        "Una vacación con estatus Tomado no puede cambiar de estado."
                });
            }

            // Actualizar estado
            vacacion.Estatus = nuevoEstatus;

            _context.SaveChanges();

            return Ok(new
            {
                mensaje = "Estatus actualizado correctamente.",
                idVacacion = vacacion.IdVacacion,
                estatus = vacacion.Estatus
            });
        }

        [HttpPost]
        public IActionResult CrearVacacion(
            [FromBody] Vacaciones dto)
        {
            try
            {
                var vacacion = new Vacaciones
                {
                    IdEmpleado = 2, // fijo por ahora
                    IdLider = 1,    // fijo por ahora

                    FechaInicio =
                        dto.FechaInicio.Date,

                    FechaFin =
                        dto.FechaFin.Date,

                    Comentarios =
                        dto.Comentarios,

                    Estatus =
                        "Calendarizado"
                };

                _context.Vacaciones.Add(vacacion);

                _context.SaveChanges();

                return Ok(new
                {
                    mensaje =
                        "Vacación creada correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        mensaje =
                            "Error al crear vacación",

                        error =
                            ex.InnerException?.Message
                            ?? ex.Message
                    }
                );
            }
        }
        
    }
}