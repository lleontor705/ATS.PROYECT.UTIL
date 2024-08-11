using System;
using System.Configuration;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public class DatabaseHandlerFactory
    {
        private readonly ConnectionStringSettings connectionStringSettings;

        public DatabaseHandlerFactory(string connectionName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
            if (connectionStringSettings == null)
            {
                throw new ArgumentException($"No se encontró la cadena de conexión con el nombre '{connectionName}' en el archivo Web.config.");
            }
        }

        public IDataAccess CrearDatabase()
        {
            IDataAccess database = null;
            switch (connectionStringSettings.ProviderName.ToLower())
            {
                case "system.data.sqlclient":
                    database = new SqlDataAccess(connectionStringSettings.ConnectionString);
                    break;
                case "npgsql":
                    database = new PostgreDataAccess(connectionStringSettings.ConnectionString);
                    break;
                case "mysql.data.mysqlclient":
                    database = new MySqlDataAccess(connectionStringSettings.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"El proveedor '{connectionStringSettings.ProviderName}' no está soportado.");
            }

            return database;
        }

        public string ObtenerNombreProveedor()
        {
            return connectionStringSettings.ProviderName;
        }
    }


}
