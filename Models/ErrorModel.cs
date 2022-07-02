namespace ApiKalumNotas.Models
{
    public class ErrorModel
    {
        public int Estatus {get;set;}
        public int CodigoError {get;set;}
        public string TipoError {get;set;}
        public string Codigo {get;set;}
        public string Descripcion {get;set;}
    }
}