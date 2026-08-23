using System.ComponentModel.DataAnnotations;

namespace MichiMarket.Models
{
    public class Producto
    {
        public int id_producto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria")]
        [StringLength(200, ErrorMessage = "Maximo 200 caracteres")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 99999.99, ErrorMessage = "Precio debe ser mayor a 0")]
        public decimal precio { get; set; }

        public bool estado { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock no puede ser negativo")]
        public int stock { get; set; }

        public string foto_producto { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(20)]
        public string tipo { get; set; }

        [Required(ErrorMessage = "El animal es obligatorio")]
        [StringLength(20)]
        public string animal { get; set; }
    }
}
