using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Data.SqlClient;
using static ATS.PROYECT.UTIL.Constant.Constants;

namespace ATS.PROYECT.UTIL.BDHelper
{
    public static class DataParameterManager
    {
        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, object valor, DbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            IDbDataParameter parametro = null;
            switch (nombreproveedor.ToLower())
            {
                case DataBaseType.SQLCLIENT:
                    parametro = CrearSqlParametro(nombre, valor, dbType, direccion);
                    break;
                case DataBaseType.POSTGRESQL:
                    parametro = CrearPostgreParametro(nombre, valor, dbType, direccion);
                    break;
                case DataBaseType.MYSQLCLIENT:
                    parametro = CrearMySqlParametro(nombre, valor, dbType, direccion);
                    break;
                default:
                    throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado.");
            }
            return parametro;
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, object valor, SqlDbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            if (nombreproveedor.ToLower() == DataBaseType.SQLCLIENT)
            {
                return CrearSqlParametro(nombre, valor, dbType, direccion);
            }
            throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado para SqlDbType.");
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, object valor, NpgsqlDbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            if (nombreproveedor.ToLower() == DataBaseType.POSTGRESQL)
            {
                return CrearPostgreParametro(nombre, valor, dbType, direccion);
            }
            throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado para NpgsqlDbType.");
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, int tamano, object valor, DbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            IDbDataParameter parametro = null;
            switch (nombreproveedor.ToLower())
            {
                case DataBaseType.SQLCLIENT:
                    parametro = CrearSqlParametro(nombre, tamano, valor, dbType, direccion);
                    break;
                case DataBaseType.POSTGRESQL:
                    parametro = CrearPostgreParametro(nombre, tamano, valor, dbType, direccion);
                    break;
                case DataBaseType.MYSQLCLIENT:
                    parametro = CrearMySqlParametro(nombre, tamano, valor, dbType, direccion);
                    break;
                default:
                    throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado.");
            }
            return parametro;
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, object valor, ParameterDirection direccion = ParameterDirection.Input)
        {
            IDbDataParameter parametro = null;
            switch (nombreproveedor.ToLower())
            {
                case DataBaseType.SQLCLIENT:
                    parametro = CrearSqlParametro(nombre, valor, direccion);
                    break;
                case DataBaseType.POSTGRESQL:
                    parametro = CrearPostgreParametro(nombre, valor, direccion);
                    break;
                case DataBaseType.MYSQLCLIENT:
                    parametro = CrearMySqlParametro(nombre, valor, direccion);
                    break;
                default:
                    throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado.");
            }
            return parametro;
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, SqlDbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            if (nombreproveedor.ToLower() == DataBaseType.SQLCLIENT)
            {
                return CrearSqlParametro(nombre, dbType, direccion);
            }
            throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado para SqlDbType.");
        }

        public static IDbDataParameter CrearParametro(string nombreproveedor, string nombre, NpgsqlDbType dbType, ParameterDirection direccion = ParameterDirection.Input)
        {
            if (nombreproveedor.ToLower() == DataBaseType.SQLCLIENT)
            {
                return CrearPostgreParametro(nombre, dbType, direccion);
            }
            throw new NotSupportedException($"El proveedor '{nombreproveedor}' no está soportado para NpgsqlDbType.");
        }

        #region SQL
        private static IDbDataParameter CrearSqlParametro(string nombre, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new SqlParameter
            {
                DbType = dbType,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearSqlParametro(string nombre, object valor, SqlDbType dbType, ParameterDirection direccion)
        {
            return new SqlParameter
            {
                SqlDbType = dbType,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearSqlParametro(string nombre, int tamano, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new SqlParameter
            {
                DbType = dbType,
                Size = tamano,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearSqlParametro(string nombre, object valor, ParameterDirection direccion)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearSqlParametro(string nombre, SqlDbType dbType, ParameterDirection direccion)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Direction = direccion,
                SqlDbType = dbType
            };
        }
        #endregion

        #region POSTGRE
        private static IDbDataParameter CrearPostgreParametro(string nombre, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new NpgsqlParameter
            {
                DbType = dbType,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearPostgreParametro(string nombre, object valor, NpgsqlDbType dbType, ParameterDirection direccion)
        {
            return new NpgsqlParameter
            {
                NpgsqlDbType = dbType,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearPostgreParametro(string nombre, int tamano, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new NpgsqlParameter
            {
                DbType = dbType,
                Size = tamano,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearPostgreParametro(string nombre, object valor, ParameterDirection direccion)
        {
            return new NpgsqlParameter
            {
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearPostgreParametro(string nombre, NpgsqlDbType dbType, ParameterDirection direccion)
        {
            return new NpgsqlParameter
            {
                ParameterName = nombre,
                Direction = direccion,
                NpgsqlDbType = dbType
            };
        }
        #endregion
        #region MYSQL
        private static IDbDataParameter CrearMySqlParametro(string nombre, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new MySqlParameter
            {
                DbType = dbType,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearMySqlParametro(string nombre, int tamano, object valor, DbType dbType, ParameterDirection direccion)
        {
            return new MySqlParameter
            {
                DbType = dbType,
                Size = tamano,
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        private static IDbDataParameter CrearMySqlParametro(string nombre, object valor, ParameterDirection direccion)
        {
            return new MySqlParameter
            {
                ParameterName = nombre,
                Direction = direccion,
                Value = valor
            };
        }

        #endregion
    }


}
