namespace MichiMarket.Models
{
    public class PedidoDetalle
    {
        public int id_pedido_detalle { get; set; }
        public int id_pedido { get; set; }
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public string foto_producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unit { get; set; }
        public decimal subtotal { get; set; }
    }
}
