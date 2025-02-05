using System.Data;
using System.Data.SqlClient;

namespace PN.SmartLib.Connection
{
    public class SQLDBConnection : BaseConnection
    {
        public SQLDBConnection(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new SqlConnection(connectionString);
        }
        public override void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
            }

            Transaction = null;
            SqlConnection.ClearPool((SqlConnection)Connection);
        }

        public override void OpenConnection()
        {
            if (!IsConnected)
            {
                if (Connection == null)
                {
                    Connection = new SqlConnection(ConnectionString);
                }

                Connection.Open();
            }
        }
        public override void FillDataSet(string query, IDataParameter[] parameters, DataSet dataSet)
        {
            using (SqlCommand selectCommand = (SqlCommand)SetCommand(query, parameters))
            {
                var adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dataSet);
            }
        }

        public override IDbCommand SetCommand(string query, IDataParameter[] parameters = null)
        {
            SqlCommand sqlCommand = ((Transaction == null) ? 
                                    new SqlCommand(query, (SqlConnection)Connection) : 
                                    new SqlCommand(query, (SqlConnection) Connection, (SqlTransaction)Transaction));
            if (parameters != null)
            {
                foreach (IDataParameter value in parameters)
                {
                    sqlCommand.Parameters.Add(value);
                }
            }

            return sqlCommand;
        }
    }
}
