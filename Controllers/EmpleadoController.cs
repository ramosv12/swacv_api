using Microsoft.AspNetCore.Mvc;
using swacv_api.Data;
using Microsoft.EntityFrameworkCore;

namespace swacv_api.Controllers
{
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