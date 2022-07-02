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
using System.Linq;
using AutoMapper;
using ApiKalumNotas.DTOs;
using System;

namespace ApiKalumNotas.Controllers
{

    [Route("/kalum-notas/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarrerasTecnicasController : ControllerBase
    {
        private readonly KalumNotasDBContext kalumNotasDBContext;
        private readonly ILogger<CarrerasTecnicasController> logger;
        private readonly IMapper mapper;
        public CarrerasTecnicasController(KalumNotasDBContext kalumNotasDBContext, ILogger<CarrerasTecnicasController> logger, IMapper mapper)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.kalumNotasDBContext = kalumNotasDBContext;
        }
        [HttpGet("{numeroPagina}", Name = "GetCarrerasTecnicasPage")]
        [Route("page/{numeroPagina}")]
        public async Task<ActionResult<IEnumerable<CarreraTecnica>>> GetCarrerasTecnicas(int numeroPagina = 0)
        {
            logger.LogDebug("Iniciando el proceso de consulta de carreras tecnicas");
            var queryable = this.kalumNotasDBContext.CarreraTecnicas.AsQueryable();
            var paginacion = new HttpResponsePaginacion<CarreraTecnica>(queryable, numeroPagina);
            if (paginacion.Content == null || paginacion.Content.Count == 0)
            {
                logger.LogWarning("No se encontraron registros en la tabla carreras tecnicas");
                return NoContent();
            }
            else
            {
                logger.LogInformation("Consulta de carreras tecncias exitosamente");
                return Ok(paginacion);
            }
        }
        [HttpGet("{carreraId,numeroPagina}", Name = "GetModulosPorCarrera")]
        [Route("{carreraId}/modulos/page/{numeroPagina}")]
        public async Task<ActionResult<IEnumerable<Modulo>>> GetModulosPorCarrera(string carreraId, int numeroPagina = 0)
        {
            logger.LogDebug("Iniciando el proceso de consulta de modulos por carrera");
            CarreraTecnica carreraTecnica = await this.kalumNotasDBContext.CarreraTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == carreraId);
            if (carreraTecnica == null)
            {
                logger.LogWarning($"No se encontro una carrera técnica con el codigo {carreraId}");
                return NoContent();
            }
            else
            {
                var modulos = await this.kalumNotasDBContext.Modulos.Where(m => m.CarreraId == carreraTecnica.CarreraId).ToListAsync();                
                if (modulos == null && modulos.Count == 0)
                {
                    logger.LogWarning("No se encontraron registros en la tabla de modulos");
                    return NoContent();
                }
                else
                {
                    logger.LogInformation("Se ejecuto exitosamente la consulta");
                    var queryable = mapper.Map<List<Modulo>,List<ModuloDTO>>(modulos).AsQueryable();                    
                    var paginacion = new HttpResponsePaginacion<ModuloDTO>(queryable, numeroPagina);
                    return Ok(paginacion);
                }
            }
        }
    }
}