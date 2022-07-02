namespace ApiKalumNotas.Models
{
    public sealed class Utils
    {
        private readonly static Utils _instance = new Utils();
        public Utils()
        {            
        }
        public static Utils Instance
        {
            get
            {
                return _instance;
            }
        }
        public ErrorModel GetError(int estatus, string descripcion){
            ErrorModel error = new ErrorModel();
            error.Estatus = estatus;
            switch(estatus) {
                case 400:
                    error.CodigoError = 400;
                    error.TipoError = "Error de cliente";
                    error.Codigo = "001";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"Bad request":descripcion;
                    break;
                case 401:
                    error.CodigoError = 401;
                    error.TipoError = "Error de cliente";
                    error.Codigo = "002";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"Invalid authorization token":descripcion;
                    break;
                case 403:
                    error.CodigoError = 403;
                    error.TipoError = "Error de negocio";
                    error.Codigo = "003";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"Business error":descripcion;
                    break;
                case 404:
                    error.CodigoError = 404;
                    error.TipoError = "Error de cliente";
                    error.Codigo = "004";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"Not found":descripcion;
                    break;
                case 500:
                    error.CodigoError = 500;
                    error.TipoError = "Error de servidor";
                    error.Codigo = "005";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"Internal server error":descripcion;
                    break;    
                case 503:
                    error.CodigoError = 503;
                    error.TipoError = "Error de comunicación";
                    error.Codigo = "006";
                    error.Descripcion = string.IsNullOrEmpty(descripcion)?"There is not communication with service":descripcion;
                    break;
            }
            return error;
        }
    }
}