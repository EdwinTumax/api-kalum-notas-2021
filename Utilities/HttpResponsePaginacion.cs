using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using ApiKalumNotas.DTOs;
using System.Collections.Generic;

namespace ApiKalumNotas.Utilities
{
    public class HttpResponsePaginacion<T> : PaginacionDTO<T>
    {
        public HttpResponsePaginacion(IQueryable<T> source, int number)
        {
            this.Number = number;
            int cantidadRegistrosPorPagina = 2;
            int totalRegistros = source.Count();    
            this.TotalPages = (int) Math.Ceiling((Double) totalRegistros / cantidadRegistrosPorPagina);            
            this.Content = source.Skip(cantidadRegistrosPorPagina * Number).Take(cantidadRegistrosPorPagina).ToList();
            if(this.Number == 0){
                this.First = true;                
            }else if((this.Number + 1) == this.TotalPages){
                this.Last = true;
            }
        }
    }
}