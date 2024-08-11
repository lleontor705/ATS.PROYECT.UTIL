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

        /// <summary>
        /// Initializes a new instance of the <see cref="DBManager"/> class.
        /// </summary>
        /// <param name="cadenaconexion">The connection string name.</param>
        public DBManager(string cadenaconexion)
        {
            dbFactory = new DatabaseHandlerFactory(cadenaconexion);
            database = dbFactory.CrearDatabase();
            nombreproveedor = dbFactory.ObtenerNombreProveedor();
        }

        /// <summary>
        /// Obtains a database connection.
        /// </summary>
        /// <returns>An open database connection.</returns>
        public IDbConnection ObtenerConexionBaseDatos()
        {
            return database.CrearConexion();
        }

        /// <summary>
        /// Closes the specified database connection.
        /// </summary>
        /// <param name="conexion">The database connection to close.</param>
        public void CerrarConexionBaseDatos(IDbConnection conexion)
        {
            database.CerrarConexion(conexion);
        }

        /// <summary>
        /// Creates a database parameter.
        /// </summary>
        /// <param name="nombre">The name of the parameter.</param>
        /// <param name="valor">The value of the parameter.</param>
        /// <param name="dbtype">The database type of the parameter.</param>
        /// <returns>The created database parameter.</returns>
        public IDbDataParameter CrearParametro(string nombre, object valor, DbType dbtype)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a SQL Server-specific database parameter.
        /// </summary>
        /// <param name="nombre">The name of the parameter.</param>
        /// <param name="valor">The value of the parameter.</param>
        /// <param name="dbtype">The SQL Server type of the parameter.</param>
        /// <returns>The created SQL Server-specific database parameter.</returns>
        public IDbDataParameter CrearParametro(string nombre, object valor, SqlDbType dbtype)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a PostgreSQL-specific database parameter.
        /// </summary>
        /// <param name="nombre">The name of the parameter.</param>
        /// <param name="valor">The value of the parameter.</param>
        /// <param name="dbtype">The PostgreSQL type of the parameter.</param>
        /// <param name="direccion">The direction of the parameter.</param>
        /// <returns>The created PostgreSQL-specific database parameter.</returns>
        public IDbDataParameter CrearParametro(string nombre, object valor, NpgsqlDbType dbtype, ParameterDirection direccion)
        {
            return DataParameterManager.CrearParametro(nombreproveedor, nombre, valor, dbtype, direccion);
        }

        /// <summary>
        /// Executes a command and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="DataTable"/> containing the results of the command.</returns>
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

        /// <summary>
        /// Executes a command and returns the result as a <see cref="DataSet"/>.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="DataSet"/> containing the results of the command.</returns>
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

        /// <summary>
        /// Executes a command and returns the result as a data reader.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="conexion">The database connection.</param>
        /// <returns>An <see cref="IDataReader"/> to read the results.</returns>
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

        /// <summary>
        /// Executes a non-query command (DELETE).
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        public void Delete(string comandotexto, CommandType commandType, IDbDataParameter[] parameters = null)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        /// <summary>
        /// Executes a non-query command (INSERT).
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        public void Insert(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        /// <summary>
        /// Executes a command and returns the identity of the last inserted record as an integer.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="lastId">The identity of the last inserted record.</param>
        /// <returns>The identity of the last inserted record.</returns>
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

        /// <summary>
        /// Executes a command and returns the identity of the last inserted record as a long integer.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="lastId">The identity of the last inserted record.</param>
        /// <returns>The identity of the last inserted record.</returns>
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

        /// <summary>
        /// Executes a command within a transaction (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        public void InsertWithTransaction(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, parameters);
        }

        /// <summary>
        /// Executes a command within a transaction with a specified isolation level.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <param name="parameters">The command parameters.</param>
        public void InsertWithTransaction(string comandotexto, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, isolationLevel, parameters);
        }

        /// <summary>
        /// Executes a non-query command (UPDATE).
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        public void Update(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteNonQuery(comandotexto, commandType, parameters);
        }

        /// <summary>
        /// Executes an UPDATE command within a transaction.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        public void UpdateWithTransaction(string comandotexto, CommandType commandType, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, parameters);
        }

        /// <summary>
        /// Executes an UPDATE command within a transaction with a specified isolation level.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <param name="parameters">The command parameters.</param>
        public void UpdateWithTransaction(string comandotexto, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            ExecuteWithTransaction(comandotexto, commandType, isolationLevel, parameters);
        }

        /// <summary>
        /// Executes a command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The first column of the first row in the result set.</returns>
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

        /// <summary>
        /// Executes a SQL query and maps the result to a list of objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to map the result to.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the mapped objects.</returns>
        public IEnumerable<T> Query<T>(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Query<T>(sql, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes a SQL query and maps the result to a single object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to map the result to.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The mapped object of type <typeparamref name="T"/>.</returns>
        public T QuerySingle<T>(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="sql">The SQL command.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute(string sql, CommandType commandType, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Execute(sql, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes a non-query SQL command within a transaction.
        /// </summary>
        /// <param name="sql">The SQL command.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
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

        /// <summary>
        /// Executes a non-query SQL command within a transaction with a specified isolation level.
        /// </summary>
        /// <param name="sql">The SQL command.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <param name="parameters">The command parameters.</param>
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

        /// <summary>
        /// Executes a non-query command within a transaction (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
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

        /// <summary>
        /// Executes a non-query command within a transaction with a specified isolation level.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <param name="parameters">The command parameters.</param>
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

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="comandotexto">The command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The command parameters.</param>
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

        /// <summary>
        /// Executes a stored procedure and maps the result to a list of objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to map the result to.</typeparam>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the mapped objects.</returns>
        public IEnumerable<T> QueryStoredProcedure<T>(string procedureName, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Executes a stored procedure and maps the result to a single object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to map the result to.</typeparam>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        /// <returns>The mapped object of type <typeparamref name="T"/>.</returns>
        public T QuerySingleStoredProcedure<T>(string procedureName, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.QuerySingleOrDefault<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Executes a stored procedure as a non-query command.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteStoredProcedure(string procedureName, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                return conexion.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Executes a stored procedure as a non-query command within a transaction.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        public void ExecuteStoredProcedureWithTransaction(string procedureName, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        conexion.Execute(procedureName, parameters, transaction, commandType: CommandType.StoredProcedure);
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

        /// <summary>
        /// Executes a stored procedure as a non-query command within a transaction with a specified isolation level.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        public void ExecuteStoredProcedureWithTransaction(string procedureName, IsolationLevel isolationLevel, object parameters = null)
        {
            using (var conexion = database.CrearConexion())
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        conexion.Execute(procedureName, parameters, transaction, commandType: CommandType.StoredProcedure);
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

}
