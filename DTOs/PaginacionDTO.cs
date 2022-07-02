using System.Collections.Generic;

namespace ApiKalumNotas.DTOs
{
    public class PaginacionDTO<T>
    {
        public int Number {get;set;}
        public int TotalPages {get;set;}
        public bool First {get;set;}        
        public bool Last {get;set;}        

        public List<T> Content {get;set;}

    }
}