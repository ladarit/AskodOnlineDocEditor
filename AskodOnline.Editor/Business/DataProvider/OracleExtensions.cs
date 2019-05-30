using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace AskodOnline.Editor.Business.DataProvider
{
    public static class OracleExtensions
    {
        private static void AddInputParam(this OracleCommand command, string name, OracleDbType dbType, object value)
        {
            command.Parameters.Add(name, dbType, value ?? DBNull.Value, ParameterDirection.Input);
        }

        public static void AddStringInputParam(this OracleCommand command, string name, object value)
        {
            command.AddInputParam(name, OracleDbType.Varchar2, value);
        }

        public static void AddDateInputParam(this OracleCommand command, string name, object value)
        {
            command.AddInputParam(name, OracleDbType.Date, value);
        }

        public static void AddInt64InputParam(this OracleCommand command, string name, object value)
        {
            command.AddInputParam(name, OracleDbType.Int64, value);
        }

        public static void AddBlobInputParam(this OracleCommand command, string name, object value)
        {
            command.AddInputParam(name, OracleDbType.Blob, value);
        }

        public static OracleParameter AddStringOutputParam(this OracleCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.Output;
            parameter.DbType = DbType.StringFixedLength;
            parameter.Size = 15000;
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static OracleParameter AddInt64OutputParam(this OracleCommand command, string name)
        {
            return command.Parameters.Add(name, OracleDbType.Int64, ParameterDirection.Output);
        }
    }
}
