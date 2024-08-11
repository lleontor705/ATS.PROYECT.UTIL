using Npgsql;
using System.Data;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public class PostgreDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public PostgreDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
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
            return new NpgsqlCommand
            {
                CommandText = textocomando,
                CommandType = tipocomando,
                Connection = (NpgsqlConnection)conexion
            };
        }

        public IDbDataAdapter CrearAdaptador(IDbCommand comando)
        {
            return new NpgsqlDataAdapter((NpgsqlCommand)comando);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, DbType dbtype, ParameterDirection direccion)
        {
            var parametro = new NpgsqlParameter
            {
                ParameterName = nombre,
                Value = valor,
                DbType = dbtype,
                Direction = direccion
            };
            return parametro;
        }
    }

}
