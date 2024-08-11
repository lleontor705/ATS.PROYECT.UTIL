using System.Data;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public interface IDataAccess
    {
        IDbConnection CrearConexion();
        void CerrarConexion(IDbConnection conexion);
        IDbCommand CrearComando(string textocomando, CommandType tipocomando, IDbConnection conexion);
        IDbDataAdapter CrearAdaptador(IDbCommand comando);
        IDbDataParameter CrearParametro(string nombre, object valor, DbType dbtype, ParameterDirection direccion);
    }
}
