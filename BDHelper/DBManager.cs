using Dapper;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public class DBManager
    {
        private readonly DatabaseHandlerFactory dbFactory;
        private readonly IDataAccess database;
        private readonly string nombreproveedor;

        public DBManager(string cadenaconexion)
        {
            dbFactory = new DatabaseHandlerFactory(cadenaconexion);
            database = dbFactory.CrearDatabase();
            nombreproveedor = dbFactory.ObtenerNombreProveedor();
        }

        // Métodos de ADO.NET tradicionales

        public IDbConnection ObtenerConexionBaseDatos()
        {
            return database.CrearConexion();
        }

        public void CerrarConexionBaseDatos(IDbConnection conexion)
        {
            database.CerrarConexion(conexion);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, DbType dbtype)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, ParameterDirection.Input);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, SqlDbType dbtype)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, ParameterDirection.Input);
        }

        public IDbDataParameter CrearParametro(string nombre, object valor, NpgsqlDbType dbtype, ParameterDirection direccion)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, direccion);
        }

        public DataTable GetDataTable(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }

                    var dataset = new DataSet();
                    var dataadapter = database.CrearAdaptador(comando);
                    dataadapter.Fill(dataset);
                    return dataset.Tables[0];
                }
            }
        }

        public DataSet GetDataSet(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }

                    var dataset = new DataSet();
                    var dataadapter = database.CrearAdaptador(comando);
                    dataadapter.Fill(dataset);
                    return dataset;
                }
            }
        }

        public IDataReader GetDataReader(string comandotexto, CommandType commandType, IDbDataParameter[] parameters, out IDbConnection conexion)
        {
            conexion = database.CrearConexion();
            conexion.Open();

            var comando = database.CrearComando(comandotexto, commandType, conexion);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    comando.Parameters.Add(parameter);
                }
            }

            return comando.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public void Delete(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        public void Insert(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        public int Insert(string comandotexto, CommandType commandType, IDbDataParameter[] parameters, out int lastId)
        {
            lastId = 0;
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }

                    object newId = comando.ExecuteScalar();
                    lastId = Convert.ToInt32(newId);
                }
            }

            return lastId;
        }

        public long Insert(string comandotexto, CommandType commandType, IDbDataParameter[] parameters, out long lastId)
        {
            lastId = 0;
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }

                    object newId = comando.ExecuteScalar();
                    lastId = Convert.ToInt64(newId);
                }
            }

            return lastId;
        }

        public void InsertWithTransaction(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, parameters);
        }

        public void InsertWithTransaction(string comandotexto, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, isolationLevel, parameters);
        }

        public void Update(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        public void UpdateWithTransaction(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, parameters);
        }

        public void UpdateWithTransaction(string comandotexto, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, isolationLevel, parameters);
        }

        public object GetScalarValue(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }

                    return comando.ExecuteScalar();
                }
            }
        }

        // Métodos utilizando Dapper para mapear a objetos

        public IEnumerable<T> Query<T>(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Query<T>(sql, parameters, commandType: commandType);
            }
        }

        public T QuerySingle<T>(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }

        public int Execute(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Execute(sql, parameters, commandType: commandType);
            }
        }

        public void ExecuteWithTransaction(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        conexion.Execute(sql, parameters, transaction, commandType: commandType);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ExecuteWithTransaction(string sql, CommandType commandType, IsolationLevel isolationLevel, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        conexion.Execute(sql, parameters, transaction, commandType: commandType);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private void ExecuteWithTransaction(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction())
                {
                    using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                    {
                        comando.Transaction = transaction;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                comando.Parameters.Add(parameter);
                            }
                        }

                        try
                        {
                            comando.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        private void ExecuteWithTransaction(string comandotexto, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction(isolationLevel))
                {
                    using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                    {
                        comando.Transaction = transaction;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                comando.Parameters.Add(parameter);
                            }
                        }

                        try
                        {
                            comando.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        private void ExecuteNonQuery(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var comando = database.CrearComando(comandotexto, commandType, conexion))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            comando.Parameters.Add(parameter);
                        }
                    }
                    comando.ExecuteNonQuery();
                }
            }
        }
    }

}
