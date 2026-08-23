using Microsoft.Data.SqlClient;
using MichiMarket.Models;
using System.Data;

namespace MichiMarket.Repositories
{
    public class CarritoRepository
    {
        private readonly Conexion _conexion;

        public CarritoRepository(Conexion conexion)
        {
            _conexion = conexion;
        }

        public void Agregar(int idUsuario, int idProducto, int cantidad)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_agregar_carrito", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_producto", idProducto);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<CarritoDetalle> Listar(int idUsuario)
        {
            var lista = new List<CarritoDetalle>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_listar_carrito", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CarritoDetalle
                            {
                                id_carrito_detalle = Convert.ToInt32(dr["id_carrito_detalle"]),
                                id_producto = Convert.ToInt32(dr["id_producto"]),
                                nombre = dr["nombre"].ToString(),
                                foto_producto = dr["foto_producto"].ToString(),
                                tipo = dr["tipo"].ToString(),
                                animal = dr["animal"].ToString(),
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

        public void Eliminar(int idUsuario, int idCarritoDetalle)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_eliminar_carrito", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_carrito_detalle", idCarritoDetalle);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarCantidad(int idUsuario, int idCarritoDetalle, int cantidad)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_actualizar_cantidad_carrito", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_carrito_detalle", idCarritoDetalle);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
