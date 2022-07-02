using System.ComponentModel.DataAnnotations;

namespace ApiKalumNotas.DTOs
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        public string Password {get;set;}
        public string Role {get;set;}
    }
}