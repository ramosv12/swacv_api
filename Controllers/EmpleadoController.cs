using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swacv_api.Data;
using swacv_api.DTOs;

namespace swacv_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/empleado")]
    public class EmpleadoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpleadoController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("dias")]
        public IActionResult GetDias()
        {
            var empleado = _context.Empleados.FirstOrDefault();

            if (empleado == null)
                return NotFound("No hay empleados");

            return Ok(new
            {
                empleado.IdEmpleado,
                empleado.DiasVacacionesDisponibles
            });
        }


        [HttpGet("control-rh")]
        public IActionResult GetControlRh()
        {
            var catalogo = _context.CatalogoVacacionesAntiguedad
                .AsNoTracking()
                .OrderBy(c => c.AniosMinimos)
                .ToList();


            var empleados = (
                from empleado in _context.Empleados.AsNoTracking()

                join usuario in _context.Usuarios.AsNoTracking()
                    on empleado.IdUsuario equals usuario.IdUsuario

                join equipo in _context.Equipos.AsNoTracking()
                    on empleado.IdEquipo equals equipo.IdEquipo
                    into equiposEmpleado

                from equipo in equiposEmpleado.DefaultIfEmpty()

                orderby usuario.Nombres, usuario.Apellidos

                select new
                {
                    empleado.IdEmpleado,

                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Email,

                    Departamento =
                        equipo != null
                            ? equipo.NombreEquipo
                            : "Sin equipo",

                    empleado.AltaIMSS,

                    empleado.DiasVacacionesDisponibles,

                    empleado.VencimientoDiasVacaciones
                }

            ).ToList();


            var resultado = empleados
                .Select(empleado =>
                {
                    int antiguedad =
                        CalcularAntiguedadAnios(
                            empleado.AltaIMSS
                        );


                    int diasVacacionesAnio =
                        catalogo
                            .Where(c =>
                                antiguedad >= c.AniosMinimos
                                &&
                                antiguedad <= c.AniosMaximos
                            )
                            .Select(c => c.DiasVacaciones)
                            .FirstOrDefault();


                    return new ControlRhEmpleadoDto
                    {
                        IdEmpleado =
                            empleado.IdEmpleado,

                        Nombres =
                            empleado.Nombres,

                        Apellidos =
                            empleado.Apellidos,

                        Email =
                            empleado.Email,

                        Departamento =
                            empleado.Departamento,

                        AltaIMSS =
                            empleado.AltaIMSS,

                        DiasVacacionesAnio =
                            diasVacacionesAnio,

                        DiasDisponibles =
                            empleado.DiasVacacionesDisponibles,

                        VencimientoVacaciones =
                            empleado.VencimientoDiasVacaciones
                    };

                })
                .ToList();


            return Ok(resultado);
        }

        [HttpPatch("{idEmpleado}/control-rh")]
        public IActionResult ActualizarControlRh(
            int idEmpleado,
            [FromBody] ActualizarEmpleadoRhDto dto
        )
        {
            var empleado = _context.Empleados
                .FirstOrDefault(
                    e => e.IdEmpleado == idEmpleado
                );

            if (empleado == null)
            {
                return NotFound(new
                {
                    mensaje = "El empleado no fue encontrado."
                });
            }


            if (
                dto.DiasVacacionesDisponibles.HasValue &&
                dto.DiasVacacionesDisponibles.Value < 0
            )
            {
                return BadRequest(new
                {
                    mensaje =
                        "Los días de vacaciones disponibles no pueden ser negativos."
                });
            }


            if (
                !dto.AltaIMSS.HasValue &&
                !dto.DiasVacacionesDisponibles.HasValue &&
                !dto.VencimientoDiasVacaciones.HasValue
            )
            {
                return BadRequest(new
                {
                    mensaje =
                        "Debe enviar al menos un dato para actualizar."
                });
            }


            if (dto.AltaIMSS.HasValue)
            {
                empleado.AltaIMSS =
                    dto.AltaIMSS.Value.Date;
            }


            if (dto.DiasVacacionesDisponibles.HasValue)
            {
                empleado.DiasVacacionesDisponibles =
                    dto.DiasVacacionesDisponibles.Value;
            }


            if (dto.VencimientoDiasVacaciones.HasValue)
            {
                empleado.VencimientoDiasVacaciones =
                    dto.VencimientoDiasVacaciones.Value.Date;
            }


            _context.SaveChanges();


            return Ok(new
            {
                mensaje =
                    "Datos del empleado actualizados correctamente.",

                idEmpleado =
                    empleado.IdEmpleado,

                altaIMSS =
                    empleado.AltaIMSS,

                diasVacacionesDisponibles =
                    empleado.DiasVacacionesDisponibles,

                vencimientoDiasVacaciones =
                    empleado.VencimientoDiasVacaciones
            });
        }

        private int CalcularAntiguedadAnios(
            DateTime? altaIMSS
        )
        {
            if (!altaIMSS.HasValue)
            {
                return 0;
            }


            DateTime fechaAlta =
                altaIMSS.Value.Date;

            DateTime hoy =
                DateTime.Today;


            if (fechaAlta > hoy)
            {
                return 0;
            }


            int antiguedad =
                hoy.Year - fechaAlta.Year;


            if (
                fechaAlta.Date >
                hoy.AddYears(-antiguedad)
            )
            {
                antiguedad--;
            }


            return Math.Max(
                antiguedad,
                0
            );
        }
    }
}