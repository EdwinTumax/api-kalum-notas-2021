using System.Collections.Generic;
using ApiKalumNotas.DTOs;
using ApiKalumNotas.Entities;
using AutoMapper;

namespace ApiKalumNotas.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AsignacionAlumno,AsignacionAlumnoDTO>();
            CreateMap<AsignacionAlumno,AsignacionAlumnoDetalleDTO>();
            CreateMap<AsignacionAlumnoDTO, AsignacionAlumno>();
            CreateMap<Alumno,AlumnoAsignacionDTO>().ConstructUsing(a => new AlumnoAsignacionDTO{NombreCompleto = $"{a.Apellidos} {a.Nombres}"});
            CreateMap<Clase,ClaseAsignacionDTO>();     
            CreateMap<Alumno, AlumnoDTO>();
            CreateMap<Modulo, ModuloDTO>();
            CreateMap<List<ModuloDTO>,List<Modulo>>();       
        }
    }
}