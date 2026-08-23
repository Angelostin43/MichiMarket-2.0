namespace MichiMarket.Models
{
    public class Pedido
    {
        public int id_pedido { get; set; }
        public int id_usuario { get; set; }
        public string cliente { get; set; }
        public decimal total { get; set; }
        public string estado { get; set; }
        public DateTime fecha_creacion { get; set; }
    }
}
