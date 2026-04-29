using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiExamenFinalAzure.Models
{
    public class UserModel
    {
  
        public int IdUsuario { get; set; }
     
        public string Nombre { get; set; }


   
        public string Email { get; set; }

        public string Pass { get; set; }

        public string Imagen { get; set; }
    }
}
