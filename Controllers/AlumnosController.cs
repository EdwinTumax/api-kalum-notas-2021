using System.Collections.Generic;
using System.Threading.Tasks;
using ApiKalumNotas.DbContexts;
using ApiKalumNotas.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ApiKalumNotas.DTOs;
using Microsoft.Data.SqlClient;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiKalumNotas.Utilities;

namespace ApiKalumNotas.Controllers
{
    [Route("/kalum-notas/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlumnosController : ControllerBase
    {
        private readonly KalumNotasDBContext kalumNotasDBContext;
        private readonly ILogger<AlumnosController> logger;
        private readonly IMapper mapper;
        public AlumnosController(KalumNotasDBContext kalumNotasDBContext, ILogger<AlumnosController> logger, IMapper mapper)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.kalumNotasDBContext = kalumNotasDBContext;
        }

        [HttpGet("{numeroPagina}",Name = "GetAlumnosPage")]
        [Route("page/{numeroPagina}")]
        public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnos(int numeroPagina = 1)
        {
            this.logger.LogDebug("Iniciando el proceso de consulta de la información de alumnos");
            var queryable = this.kalumNotasDBContext.Alumnos.AsQueryable();
            var paginacion = new HttpResponsePaginacion<Alumno>(queryable,numeroPagina);            
            if (paginacion.Content == null || paginacion.Content.Count == 0)
            {
                this.logger.LogWarning("No se encontraron registros en la tabla alumnos");
                return new NoContentResult();
            }            
            this.logger.LogInformation("La consulta se realizo exitosamente");
            return Ok(paginacion);
        }

        [HttpGet("{carne}", Name = "GetAlumno")]
        [AllowAnonymous]
        public async Task<ActionResult<Alumno>> GetAlumno(string carne)
        {
            this.logger.LogDebug("Iniciando el proceso de la consulta de alumno por carné");
            var alumno = await this.kalumNotasDBContext.Alumnos.FirstOrDefaultAsync(a => a.Carne == carne);
            if (alumno == null)
            {
                logger.LogWarning($"No existe el alumno con el carné {carne}");
                return NotFound();
            }
            else
            {
                logger.LogInformation("Consulta ejecutada exitosamente");
                return Ok(alumno);
            }
        }
        [HttpGet("filter")]
        public async Task<ActionResult<Alumno>> GetAlumnoByType(string parametro, string tipo)
        {
            Alumno alumno = null;
            this.logger.LogDebug("Iniciando el proceso de la consulta de alumno por tipo");
            if(tipo.Equals("carne")) {
                alumno = await this.kalumNotasDBContext.Alumnos.FirstOrDefaultAsync(a => a.Carne == parametro);
            } else if(tipo.Equals("email")) {
                alumno = await this.kalumNotasDBContext.Alumnos.FirstOrDefaultAsync(a => a.Email == parametro);
            }
            if (alumno == null)
            {
                logger.LogWarning($"No existe el alumno con el tipo {tipo}");
                return NotFound();
            }
            else
            {
                logger.LogInformation("Consulta ejecutada exitosamente");
                return Ok(alumno);
            }
        }
        [HttpGet("{carne}/asignaciones/page/{numeroPagina}")]        
        public async Task<ActionResult<AsignacionAlumnoDetalleDTO>> GetAsignacionesPorCarne(string carne, int numeroPagina = 0)
        {
            Alumno alumno = null;
            this.logger.LogDebug($"Iniciando el proceso de la consulta de alumno con número de carné {carne}");
            alumno = await this.kalumNotasDBContext.Alumnos.FirstOrDefaultAsync(a => a.Carne == carne);
            if(alumno == null)
            {
                logger.LogWarning($"No existe el alumno con el carné {carne}");
                return NotFound();
            }
            else
            {
                logger.LogDebug($"Iniciando el proceso de la consulta de asignaciones con el carne = {carne}");
                var queryable = this.kalumNotasDBContext.AsignacionesAlumnos.Include(a => a.Alumno).Include(a => a.Clase).Where(c => c.Alumno.Carne == carne).AsQueryable();
                var paginacion = new HttpResponsePaginacion<AsignacionAlumno>(queryable,numeroPagina);                
                if(paginacion.Content == null || paginacion.Content.Count == 0)
                {
                    logger.LogWarning($"No existen asignaciones para el alumno con el carné {carne}");
                    return NoContent();
                }

                var asignaciones = await this.kalumNotasDBContext.AsignacionesAlumnos.Include(a => a.Alumno).Include(a => a.Clase).Where(c => c.Alumno.Carne == carne).ToListAsync();
                if(asignaciones != null && asignaciones.Count == 0)
                {
                    logger.LogWarning($"No existen registros de asignaciones para el alumno con el carné {carne}");     
                    return NotFound();
                }
                else
                {
                    //List<AsignacionAlumnoDetalleDTO> asignacionAlumnoDTOs = mapper.Map<List<AsignacionAlumnoDetalleDTO>>(asignaciones);    
                    logger.LogInformation("Se ejecuto exitosamente la consulta");
                    return Ok(paginacion);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult<AlumnoDTO>> Post([FromBody] AlumnoCreateDTO value)
        {
            logger.LogDebug("Iniciando el proceso para la creación de un nuevo alumno");
            logger.LogDebug("Iniciando el proceso de la llamada del sp_registrar_alumno ");
            AlumnoDTO alumnoDTO = null;
            var ApellidosParameter = new SqlParameter("@Apellidos", value.Apellidos);
            var NombresParameter = new SqlParameter("@Nombres", value.Nombres);
            var EmailParameter = new SqlParameter("@Email", value.Email);
            var Resultado = await this.kalumNotasDBContext.Alumnos
                                                .FromSqlRaw("sp_registrar_alumno @Apellidos, @Nombres, @Email", 
                                                    ApellidosParameter, NombresParameter, EmailParameter)
                                                .ToListAsync();
            logger.LogDebug($"Resultado de procedimiento almacenado ${Resultado}");
            if(Resultado.Count == 0)
            {
                return NoContent();
            }
            foreach (Object registro in Resultado)
            {
                alumnoDTO = mapper.Map<AlumnoDTO>(registro);
            }
            return new CreatedAtRouteResult("GetAlumno", new { carne = alumnoDTO.Carne}, alumnoDTO);
        }

    }
}