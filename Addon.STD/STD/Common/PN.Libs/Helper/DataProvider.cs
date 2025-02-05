using PN.SmartLib.Connection;
using System.Collections;

namespace PN.SmartLib.Helper
{
    public static class DataProvider
    {
        public static Hashtable[] QueryList(BaseConnection connection, string query)
        {
            Hashtable[] datas;
            datas = connection.ExecQueryToArrayHashtable(query);
            connection.Dispose();
            return datas;
        }
        public static Hashtable QuerySingle(BaseConnection connection, string query)
        {
            Hashtable data;
            data = connection.ExecQueryToHashtable(query);
            connection.Dispose();
            return data;
        }

        public static int ExecuteNonQuery(BaseConnection connection, string query)
        {
            int ret; ret = connection.ExecuteWithOpenClose(query);
            connection.Dispose();
            return ret;
        }
    }
}
