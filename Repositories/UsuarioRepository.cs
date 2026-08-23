using Microsoft.Data.SqlClient;
using MichiMarket.Models;
using System.Data;

namespace MichiMarket.Repositories
{
    public class UsuarioRepository
    {
        private readonly Conexion _conexion;

        public UsuarioRepository(Conexion conexion)
        {
            _conexion = conexion;
        }

        public Usuario IniciarSesion(string correo, string password)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_iniciar_sesion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Usuario
                            {
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                nombres = dr["nombres"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                correo = dr["correo"].ToString(),
                                foto_perfil = dr["foto_perfil"].ToString(),
                                rol = dr["rol"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_obtener_usuario_por_correo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@correo", correo);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Usuario
                            {
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                nombres = dr["nombres"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                correo = dr["correo"].ToString(),
                                password = dr["password"].ToString(),
                                foto_perfil = dr["foto_perfil"].ToString(),
                                rol = dr["rol"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_listar_usuarios", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                nombres = dr["nombres"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                correo = dr["correo"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"]),
                                foto_perfil = dr["foto_perfil"].ToString(),
                                rol = dr["rol"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Usuario ObtenerPorId(int id)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_buscar_usuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Usuario
                            {
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                nombres = dr["nombres"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                correo = dr["correo"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"]),
                                foto_perfil = dr["foto_perfil"].ToString(),
                                rol = dr["rol"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Guardar(Usuario u)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_guardar_usuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", u.id_usuario);
                    cmd.Parameters.AddWithValue("@nombres", u.nombres);
                    cmd.Parameters.AddWithValue("@apellidos", u.apellidos);
                    cmd.Parameters.AddWithValue("@correo", u.correo);
                    cmd.Parameters.AddWithValue("@password", u.password);
                    cmd.Parameters.AddWithValue("@foto_perfil", u.foto_perfil ?? "");
                    cmd.Parameters.AddWithValue("@rol", u.rol ?? "cliente");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_admin_eliminar_usuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    cmd.Parameters.AddWithValue("@rol", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
