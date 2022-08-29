using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDB.Connectors
{
    public class XDBMySqlConnector
    {
        private MySqlConnection _connection;

        public XDBMySqlConnector(string cnnStr = "")
        {
            OpenConnection(cnnStr);
        }

        ~XDBMySqlConnector()
        {
            CloseConnection();
        }

        private void OpenConnection(string cnnStr)
        {
            try
            {
                _connection = new MySqlConnection(cnnStr);
                if (_connection.State == ConnectionState.Closed) _connection.Open();
            }
            catch (MySqlException e)
            {
                throw e;
            }
        }

        private void CloseConnection()
        {
            if (_connection == null) return;
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            else _connection.Dispose();
        }

        public void CloseAllConnections()
        {
            CloseConnection();
        }

        public object ExecCommand(string mapKey, List<DbParam> values)
        {
            var query = XDBConfigurator.GetMap(mapKey).query;

            var comm = _connection.CreateCommand();

            if (values != null)
            {
                foreach (var v in values)
                {
                    var p = new MySqlParameter(v.key, MySqlDbType.VarChar)
                    {
                        Direction = ParameterDirection.Input,
                        Value = v.value
                    };

                    comm.Parameters.Add(p);
                }
            }

            comm.CommandText = query;

            dynamic res = null;

            try
            {
                var reader = comm.ExecuteReader();

                res = new List<Dictionary<string, object>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader[i] == DBNull.Value ? null : reader[i]);
                    }

                    res.Add(row);
                }

                reader.Close();
            }
            catch (MySqlException ex)
            {
                CloseConnection();
                throw ex;
            }

            CloseConnection();

            return res;
        }
    }
}