using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiKalumNotas.Entities
{
    public class Modulo
    {
        public string ModuloId {get;set;}
        public string CarreraId {get;set;}
        public string NombreModulo {get;set;}
        public int NumeroSeminarios {get;set;}        
        public virtual CarreraTecnica CarreraTecnica {get;set;}
    }
}