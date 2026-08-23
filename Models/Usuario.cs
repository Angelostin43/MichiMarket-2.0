using System.ComponentModel.DataAnnotations;

namespace MichiMarket.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "Maximo 30 caracteres")]
        public string nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30, ErrorMessage = "Maximo 30 caracteres")]
        public string apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no valido")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string correo { get; set; }

        [Required(ErrorMessage = "La contrasena es obligatoria")]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "Minimo 4 caracteres")]
        public string password { get; set; }

        public bool estado { get; set; }

        public string foto_perfil { get; set; }

        [StringLength(20)]
        public string rol { get; set; }
    }
}
