using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swacv_api.Data;

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
    }
}