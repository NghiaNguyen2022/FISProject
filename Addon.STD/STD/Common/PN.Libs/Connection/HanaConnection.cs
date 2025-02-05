using Sap.Data.Hana;
using System.Data;

namespace GT.Libs.Connection
{
    public class HanaDBConnection : BaseConnection
    {
        public HanaDBConnection(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new HanaConnection(connectionString);
        }
        public override void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
            }

            Transaction = null;
            HanaConnection.ClearPool((HanaConnection)Connection);
        }
        
        public override void FillDataSet(string query, IDataParameter[] parameters, DataSet dataSet)
        {
            using (HanaCommand selectCommand = (HanaCommand)SetCommand(query, parameters))
            {
                var adapter = new HanaDataAdapter(selectCommand);
                adapter.Fill(dataSet);
            }
        }

        public override void OpenConnection()
        {
            if (!IsConnected)
            {
                if (Connection == null)
                {
                    Connection = new HanaConnection(ConnectionString);
                }

                Connection.Open();
            }
        }

        public override IDbCommand SetCommand(string query, IDataParameter[] parameters = null)
        {
            HanaCommand sqlCommand = ((Transaction == null) ?
                                     new HanaCommand(query, (HanaConnection)Connection) :
                                     new HanaCommand(query, (HanaConnection)Connection, (HanaTransaction)Transaction));
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
