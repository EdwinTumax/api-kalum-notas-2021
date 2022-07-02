using System.Collections.Generic;
using System.Threading.Tasks;
using ApiKalumNotas.DbContexts;
using ApiKalumNotas.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiKalumNotas.Utilities;

namespace ApiKalumNotas.Controllers
{
    [Route("/kalum-notas/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClasesController : ControllerBase
    {
        private readonly KalumNotasDBContext kalumNotasDBContext;
        private readonly ILogger<ClasesController> logger;
        public ClasesController(KalumNotasDBContext kalumNotasDBContext, ILogger<ClasesController> logger)
        {
            this.logger = logger;
            this.kalumNotasDBContext = kalumNotasDBContext;
        }
        [HttpGet("page/{numeroPagina}", Name="GetClases")]
        public async Task<ActionResult<IEnumerable<Clase>>> GetClases(int numeroPagina = 0)
        {
            logger.LogDebug("Iniciando el proceso de consulta de clases");
            var queryable = this.kalumNotasDBContext.Clases.AsQueryable();
            var paginacion = new HttpResponsePaginacion<Clase>(queryable,numeroPagina);
            //var clases = await this.kalumNotasDBContext.Clases.ToListAsync();
            if(paginacion.Content == null || paginacion.Content.Count == 0)
            {   
                logger.LogWarning("No se encontraron registros en la tabla clases");
                return NoContent();
            }
            else
            {
                logger.LogInformation("Consulta de clases exitosamente");
                return Ok(paginacion);
            }
        }

        public async Task<ActionResult<IEnumerable<Clase>>> GetClasesSinPaginar(int numeroPagina)
        {
            logger.LogDebug("Iniciando el proceso de consulta de clases");
            var queryable = this.kalumNotasDBContext.Clases.AsQueryable();
            var paginacion = new HttpResponsePaginacion<Clase>(queryable,numeroPagina);
            //var clases = await this.kalumNotasDBContext.Clases.ToListAsync();
            if(paginacion.Content == null || paginacion.Content.Count == 0)
            {   
                logger.LogWarning("No se encontraron registros en la tabla clases");
                return NoContent();
            }
            else
            {
                logger.LogInformation("Consulta de clases exitosamente");
                return Ok(paginacion);
            }
        }

        [HttpGet("{claseId}", Name = "GetClase")]
        public async Task<ActionResult<Clase>> GetClase(string claseId)
        {
            logger.LogDebug($"Iniciando el proceso de la consulta de la clase con id = {claseId}");
            var clase = await this.kalumNotasDBContext.Clases.Include(c => c.Instructor).Include(c => c.Salon).Include(c => c.Horario).Include(c => c.CarreraTecnica).FirstOrDefaultAsync(c => c.ClaseId == claseId);
            if(clase == null)
            {
                logger.LogWarning($"No existe la clase con el id = {claseId}");
                return NotFound();
            }
            else
            {
                logger.LogInformation("Consulta ejecutada exisamente");
                return Ok(clase);
            }
        }
    }
}