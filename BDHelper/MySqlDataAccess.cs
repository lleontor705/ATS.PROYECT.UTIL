using MySql.Data.MySqlClient;
using System.Data;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public class MySqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public MySqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CrearConexion()
        {
            return new MySqlConnection(_connectionString);
        }

        public void CerrarConexion(IDbConnection conexion)
        {
            if (conexion != null && conexion.State != ConnectionState.Closed)
            {
                conexion.Close();
                conexion.Dispose();
            }
        }

        public IDbCommand CrearComando(string textocomando, CommandType tipocomando, IDbConnection conexion)
        {
            return new MySqlCommand
            {
                CommandText = textocomando,
                CommandType = tipocomando,
                Connection = (MySqlConnection)conexion
            };
        }

        public IDbDataAdapter CrearAdaptador(IDbCommand comando)
        {
            return new MySqlDataAdapter((MySqlCommand)comando);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new MySqlParameter
            {
                ParameterName = nombre,
                Value = valor,
                DbType = dbType,
                Direction = direccion
            };
        }
    }

}
