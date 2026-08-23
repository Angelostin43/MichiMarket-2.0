using System.ComponentModel.DataAnnotations;

namespace MichiMarket.Models
{
    public class CarritoDetalle
    {
        public int id_carrito_detalle { get; set; }
        public int id_carrito { get; set; }
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public string foto_producto { get; set; }
        public string tipo { get; set; }
        public string animal { get; set; }

        [Range(1, 100, ErrorMessage = "Cantidad debe ser entre 1 y 100")]
        public int cantidad { get; set; }

        public decimal precio_unit { get; set; }
        public decimal subtotal { get; set; }
    }
}
