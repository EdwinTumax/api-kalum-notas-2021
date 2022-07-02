using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiKalumNotas.Entities
{
    public class CarreraTecnica
    {
        public string CarreraId {get;set;}
        public string Nombre {get;set;}
        public virtual List<Clase> Clases {get;set;}
        
        public virtual List<Modulo> Modulos {get;set;}

        public override string ToString()
        {
            return this.Nombre;
        }       
    }
}