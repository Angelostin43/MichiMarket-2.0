using Microsoft.Data.SqlClient;

namespace MichiMarket.Repositories
{
    public class Conexion
    {
        private readonly string _cadena;

        public Conexion(IConfiguration configuration)
        {
            _cadena = configuration.GetConnectionString("ConexionDB");
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadena);
        }
    }
}
