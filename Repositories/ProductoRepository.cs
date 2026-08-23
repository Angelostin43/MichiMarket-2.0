using Microsoft.Data.SqlClient;
using MichiMarket.Models;
using System.Data;

namespace MichiMarket.Repositories
{
    public class ProductoRepository
    {
        private readonly Conexion _conexion;

        public ProductoRepository(Conexion conexion)
        {
            _conexion = conexion;
        }

        public List<Producto> Listar()
        {
            var lista = new List<Producto>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_listar_productos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                id_producto = Convert.ToInt32(dr["id_producto"]),
                                nombre = dr["nombre"].ToString(),
                                descripcion = dr["descripcion"].ToString(),
                                precio = Convert.ToDecimal(dr["precio"]),
                                estado = Convert.ToBoolean(dr["estado"]),
                                stock = Convert.ToInt32(dr["stock"]),
                                foto_producto = dr["foto_producto"].ToString(),
                                tipo = dr["tipo"].ToString(),
                                animal = dr["animal"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Producto> Buscar(string nombre, string tipo, string animal, decimal? precioMin, decimal? precioMax)
        {
            var lista = new List<Producto>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_buscar_productos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", (object)nombre ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipo", (object)tipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@animal", (object)animal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio_min", (object)precioMin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio_max", (object)precioMax ?? DBNull.Value);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                id_producto = Convert.ToInt32(dr["id_producto"]),
                                nombre = dr["nombre"].ToString(),
                                descripcion = dr["descripcion"].ToString(),
                                precio = Convert.ToDecimal(dr["precio"]),
                                estado = Convert.ToBoolean(dr["estado"]),
                                stock = Convert.ToInt32(dr["stock"]),
                                foto_producto = dr["foto_producto"].ToString(),
                                tipo = dr["tipo"].ToString(),
                                animal = dr["animal"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Producto ObtenerPorId(int id)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_obtener_producto_por_id", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Producto
                            {
                                id_producto = Convert.ToInt32(dr["id_producto"]),
                                nombre = dr["nombre"].ToString(),
                                descripcion = dr["descripcion"].ToString(),
                                precio = Convert.ToDecimal(dr["precio"]),
                                estado = Convert.ToBoolean(dr["estado"]),
                                stock = Convert.ToInt32(dr["stock"]),
                                foto_producto = dr["foto_producto"].ToString(),
                                tipo = dr["tipo"].ToString(),
                                animal = dr["animal"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Guardar(Producto p)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_guardar_producto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", p.id_producto);
                    cmd.Parameters.AddWithValue("@nombre", p.nombre);
                    cmd.Parameters.AddWithValue("@descripcion", p.descripcion);
                    cmd.Parameters.AddWithValue("@precio", p.precio);
                    cmd.Parameters.AddWithValue("@estado", p.estado);
                    cmd.Parameters.AddWithValue("@stock", p.stock);
                    cmd.Parameters.AddWithValue("@foto_producto", p.foto_producto ?? "");
                    cmd.Parameters.AddWithValue("@tipo", p.tipo);
                    cmd.Parameters.AddWithValue("@animal", p.animal);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_eliminar_producto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", id);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EliminarVarios(string ids)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_eliminar_productos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_productos", ids);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
