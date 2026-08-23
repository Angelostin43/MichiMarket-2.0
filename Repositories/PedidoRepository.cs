using Microsoft.Data.SqlClient;
using MichiMarket.Models;
using System.Data;

namespace MichiMarket.Repositories
{
    public class PedidoRepository
    {
        private readonly Conexion _conexion;

        public PedidoRepository(Conexion conexion)
        {
            _conexion = conexion;
        }

        public string RealizarPedido(int idUsuario)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_realizar_pedido", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return dr["mensaje"].ToString();
                        }
                    }
                }
            }
            return null;
        }

        public List<Pedido> ListarMisPedidos(int idUsuario)
        {
            var lista = new List<Pedido>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_listar_mis_pedidos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pedido
                            {
                                id_pedido = Convert.ToInt32(dr["id_pedido"]),
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                total = Convert.ToDecimal(dr["total"]),
                                estado = dr["estado"].ToString(),
                                fecha_creacion = Convert.ToDateTime(dr["fecha_creacion"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<PedidoDetalle> DetallePedido(int idUsuario, int idPedido)
        {
            var lista = new List<PedidoDetalle>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_detalle_pedido", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_pedido", idPedido);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PedidoDetalle
                            {
                                id_pedido_detalle = Convert.ToInt32(dr["id_pedido_detalle"]),
                                id_producto = Convert.ToInt32(dr["id_producto"]),
                                nombre = dr["nombre"].ToString(),
                                foto_producto = dr["foto_producto"].ToString(),
                                cantidad = Convert.ToInt32(dr["cantidad"]),
                                precio_unit = Convert.ToDecimal(dr["precio_unit"]),
                                subtotal = Convert.ToDecimal(dr["subtotal"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Pedido> ListarTodos()
        {
            var lista = new List<Pedido>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_listar_pedidos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pedido
                            {
                                id_pedido = Convert.ToInt32(dr["id_pedido"]),
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                cliente = dr["cliente"].ToString(),
                                total = Convert.ToDecimal(dr["total"]),
                                estado = dr["estado"].ToString(),
                                fecha_creacion = Convert.ToDateTime(dr["fecha_creacion"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void ActualizarEstado(int idPedido, string estado)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_actualizar_estado_pedido", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pedido", idPedido);
                    cmd.Parameters.AddWithValue("@estado", estado);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<PedidoDetalle> DetallePedidoAdmin(int idPedido)
        {
            var lista = new List<PedidoDetalle>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT pd.id_pedido_detalle, p.nombre, p.foto_producto, pd.cantidad, pd.precio_unit, pd.subtotal
                    FROM tb_pedido_detalle pd
                    INNER JOIN tb_producto p ON pd.id_producto = p.id_producto
                    WHERE pd.id_pedido = @id_pedido", conn))
                {
                    cmd.Parameters.AddWithValue("@id_pedido", idPedido);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PedidoDetalle
                            {
                                id_pedido_detalle = Convert.ToInt32(dr["id_pedido_detalle"]),
                                nombre = dr["nombre"].ToString(),
                                foto_producto = dr["foto_producto"].ToString(),
                                cantidad = Convert.ToInt32(dr["cantidad"]),
                                precio_unit = Convert.ToDecimal(dr["precio_unit"]),
                                subtotal = Convert.ToDecimal(dr["subtotal"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
