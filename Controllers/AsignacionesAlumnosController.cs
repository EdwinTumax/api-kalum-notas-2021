using System.Collections.Generic;
using System.Threading.Tasks;
using ApiKalumNotas.DbContexts;
using ApiKalumNotas.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using ApiKalumNotas.DTOs;
using AutoMapper;
using System.Linq;
using ApiKalumNotas.Models;
using ApiKalumNotas.Utilities;

namespace ApiKalumNotas.Controllers
{
    [Route("/kalum-notas/v1/Asignaciones")]
    [ApiController]
    public class AsignacionesAlumnosController : ControllerBase
    {
        private readonly KalumNotasDBContext kalumNotasDbContext;
        private readonly ILogger<AsignacionesAlumnosController> logger;
        private readonly IMapper mapper;
        public AsignacionesAlumnosController(KalumNotasDBContext kalumNotasDbContext, ILogger<AsignacionesAlumnosController> logger,
             IMapper mapper)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.kalumNotasDbContext = kalumNotasDbContext;
        }

        [HttpGet("{numeroPagina}", Name = "GetAsignacionAlumnoDetallePage")]
        [Route("page/{numeroPagina}")]
        public async Task<ActionResult<IEnumerable<AsignacionAlumnoDetalleDTO>>> GetAsignaciones(int numeroPagina = 0 )
        {
            logger.LogDebug("Iniciando el proceso para obtener el listado de las asignaciones");
            var queryable = this.kalumNotasDbContext.AsignacionesAlumnos.Include(a => a.Alumno).Include(c => c.Clase).AsQueryable();
            var paginacion = new HttpResponsePaginacion<AsignacionAlumno>(queryable,numeroPagina);
            if(paginacion.Content == null || paginacion.Content.Count == 0)
            {
                logger.LogWarning("No existen registros en la tabla Asignaciones de alumnos");
                return NoContent();
            }
            logger.LogInformation("Obteniendo el listado de asignaciones");
            return Ok(paginacion);

            /*var asignaciones = await this.kalumNotasDbContext.AsignacionesAlumnos.Include(a => a.Alumno).Include(c => c.Clase).ToListAsync();
            if (asignaciones == null || asignaciones.Count == 0)
            {
                logger.LogWarning("No existen registros en la tabla Asignaciones de alumnos");
                return NoContent();
            }
            else
            {
                List<AsignacionAlumnoDetalleDTO> asignacionAlumnoDTOs = mapper.Map<List<AsignacionAlumnoDetalleDTO>>(asignaciones);                
                logger.LogInformation("Obteniendo el listado de asignaciones");
                return Ok(asignacionAlumnoDTOs);
            }*/
        }
        [HttpGet("{asignacionId}", Name = "GetAsignacion")]
        public async Task<ActionResult<AsignacionAlumnoDetalleDTO>> GetAsignacion(string asignacionId)
        {
            logger.LogDebug($"Iniciando el proceso de la consulta de la asignación con el id = {asignacionId}");
            var asignacion = await this.kalumNotasDbContext.AsignacionesAlumnos.Include(a => a.Alumno).Include(a => a.Clase).FirstOrDefaultAsync(c => c.AsignacionId == asignacionId);
            if (asignacion == null)
            {
                logger.LogWarning($"La asignación con el id {asignacionId} no existe");
                return NotFound();
            }
            else
            {
                var asignacionAlumnoDTO = mapper.Map<AsignacionAlumnoDetalleDTO>(asignacion);
                logger.LogInformation("Se ejecuto exitosamente la consulta");
                return Ok(asignacionAlumnoDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AsignacionAlumnoDetalleDTO>> PostAsignacion([FromBody] AsignacionAlumnoDTO nuevaAsignacion)
        {
            logger.LogDebug("Iniciando el proceso de una nueva asignación");
            logger.LogDebug($"Realizando la consulta del alumno con el carné {nuevaAsignacion.Carne}");
            Alumno alumno = await this.kalumNotasDbContext.Alumnos.FirstOrDefaultAsync(a => a.Carne == nuevaAsignacion.Carne);
            if (alumno == null)
            {
                logger.LogInformation($"No existe el alumno con el carné {nuevaAsignacion.Carne}");
                return BadRequest();
            }
            logger.LogDebug($"Realizando la consulta de la clase con el id {nuevaAsignacion.ClaseId}");
            Clase clase = await this.kalumNotasDbContext.Clases.FirstOrDefaultAsync(c => c.ClaseId == nuevaAsignacion.ClaseId);
            if (clase == null)
            {
                logger.LogInformation($"No existe la clase con el id {nuevaAsignacion.ClaseId}");
                return BadRequest();
            }
            nuevaAsignacion.AsignacionId = Guid.NewGuid().ToString();
            var asignacion = mapper.Map<AsignacionAlumno>(nuevaAsignacion);
            await this.kalumNotasDbContext.AsignacionesAlumnos.AddAsync(asignacion);
            await this.kalumNotasDbContext.SaveChangesAsync();
            return new CreatedAtRouteResult("GetAsignacion", new { asignacionId = nuevaAsignacion.AsignacionId }, 
                mapper.Map<AsignacionAlumnoDetalleDTO>(asignacion));
        }
        [HttpPut("{asignacionId}")]
        public async Task<ActionResult> Put(string asignacionId, [FromBody] AsignacionAlumnoDTO ActualizarAsignacion)
        {
            //throw new NotImplementedException();
            logger.LogDebug($"Inicio del proceso de modificación de una asignación con el id {asignacionId}");
            AsignacionAlumno asignacion = await this.kalumNotasDbContext.AsignacionesAlumnos.FirstOrDefaultAsync(a => a.AsignacionId == asignacionId);
            if (asignacion == null)
            {
                logger.LogInformation($"No existe la asignación con el id {asignacionId}");
                return NotFound(Utils.Instance.GetError(404,$"No existe la asignación con el id {asignacionId}"));
            }
            else
            {
                logger.LogDebug($"Realizando la consulta del alumno con el carné {ActualizarAsignacion.Carne}");
                logger.LogDebug($"{ActualizarAsignacion}");
                Alumno alumno = await this.kalumNotasDbContext.Alumnos.FirstOrDefaultAsync(a => a.Carne == ActualizarAsignacion.Carne);
                if (alumno == null)
                {
                    logger.LogInformation($"No existe el alumno con el carné {ActualizarAsignacion.Carne}");
                    return BadRequest(Utils.Instance.GetError(400,$"No existe el alumno con el carné {ActualizarAsignacion.Carne}"));
                }
                logger.LogDebug($"Realizando la consulta de la clase con el id {ActualizarAsignacion.ClaseId}");
                Clase clase = await this.kalumNotasDbContext.Clases.FirstOrDefaultAsync(c => c.ClaseId == ActualizarAsignacion.ClaseId);
                if (clase == null)
                {
                    logger.LogInformation($"No existe la clase con el id {ActualizarAsignacion.ClaseId}");
                    return BadRequest(Utils.Instance.GetError(400,$"No existe la clase con el id {ActualizarAsignacion.ClaseId}"));
                }
                asignacion.Carne = ActualizarAsignacion.Carne;
                asignacion.ClaseId = ActualizarAsignacion.ClaseId;
                asignacion.FechaAsignacion = ActualizarAsignacion.FechaAsignacion;
                this.kalumNotasDbContext.Entry(asignacion).State = EntityState.Modified;
                await this.kalumNotasDbContext.SaveChangesAsync();
                logger.LogInformation("Los datos de la asignación fueron actualizados exitosamente");
                return NoContent();
            }
        }
        [HttpDelete("{asignacionId}")]
        public async Task<ActionResult<AsignacionAlumnoDTO>> DeleteAsignacion(String asignacionId)
        {
            logger.LogDebug("Iniciando el proceso de eliminación de la asignación");
            AsignacionAlumno asignacion = await this.kalumNotasDbContext.AsignacionesAlumnos.FirstOrDefaultAsync(a => a.AsignacionId == asignacionId);
            if (asignacion == null)
            {
                logger.LogInformation($"No existe la asignación con el Id {asignacionId}");
                return NotFound();
            }
            else
            {
                this.kalumNotasDbContext.AsignacionesAlumnos.Remove(asignacion);
                await this.kalumNotasDbContext.SaveChangesAsync();
                logger.LogInformation($"Se ha realizado la eliminación del registro con id {asignacionId}");
                return mapper.Map<AsignacionAlumnoDTO>(asignacion);
            }
        }

    }
}