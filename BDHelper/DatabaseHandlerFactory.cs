using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using static ATS.PROYECT.UTIL.Constant.Constants;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public class DatabaseHandlerFactory
    {
        private readonly ConnectionStringSettings connectionStringSettings;
        private readonly string providerName;

        // Constructor para .NET Framework (Web.config / App.config)
        /// <summary>
        /// Constructor para .NET Framework (Web.config / App.config)
        /// </summary>
        /// <param name="connectionName"></param>
        /// <exception cref="ArgumentException"></exception>
        public DatabaseHandlerFactory(string connectionName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
            if (connectionStringSettings == null)
            {
                throw new ArgumentException($"No se encontró la cadena de conexión con el nombre '{connectionName}' en el archivo Web.config o App.config.");
            }

            providerName = connectionStringSettings.ProviderName;
        }

        // Constructor para .NET Core (appsettings.json)
        /// <summary>
        /// Constructor para .NET Core (appsettings.json)
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="configuration"></param>
        /// <param name="useNetCoreConfig"></param>
        /// <exception cref="ArgumentException"></exception>
        public DatabaseHandlerFactory(string connectionName, IConfiguration configuration, bool useNetCoreConfig)
        {
            if (useNetCoreConfig)
            {
                var connectionString = configuration.GetConnectionString(connectionName);
                providerName = configuration.GetSection($"ConnectionStrings:{connectionName}:ProviderName").Value;

                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(providerName))
                {
                    throw new ArgumentException($"No se encontró la cadena de conexión o el proveedor con el nombre '{connectionName}' en el archivo appsettings.json.");
                }

                connectionStringSettings = new ConnectionStringSettings(connectionName, connectionString, providerName);
            }
            else
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
                if (connectionStringSettings == null)
                {
                    throw new ArgumentException($"No se encontró la cadena de conexión con el nombre '{connectionName}' en el archivo Web.config o App.config.");
                }

                providerName = connectionStringSettings.ProviderName;
            }
        }

        public IDataAccess CrearDatabase()
        {
            IDataAccess database = null;
            switch (providerName.ToLower())
            {
                case DataBaseType.SQLCLIENT:
                    database = new SqlDataAccess(connectionStringSettings.ConnectionString);
                    break;
                case DataBaseType.POSTGRESQL:
                    database = new PostgreDataAccess(connectionStringSettings.ConnectionString);
                    break;
                case DataBaseType.MYSQLCLIENT:
                    database = new MySqlDataAccess(connectionStringSettings.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"El proveedor '{providerName}' no está soportado.");
            }

            return database;
        }

        public string ObtenerNombreProveedor()
        {
            return providerName;
        }
    }

}
