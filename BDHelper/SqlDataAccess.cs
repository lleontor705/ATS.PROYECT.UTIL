using System.Data;
using System.Data.SqlClient;

namespace ATS.PROYECT.UTIL.BDHelper
{
    internal class SqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CrearConexion()
        {
            return new SqlConnection(_connectionString);
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
            return new SqlCommand
            {
                CommandText = textocomando,
                CommandType = tipocomando,
                Connection = (SqlConnection)conexion
            };
        }

        public IDbDataAdapter CrearAdaptador(IDbCommand comando)
        {
            return new SqlDataAdapter((SqlCommand)comando);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, DbType dbtype, ParameterDirection direccion)
        {
            var parametro = new SqlParameter
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
