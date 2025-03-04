using SAPCore.Config;
using System.Collections;

namespace ERPService.DataReader
{
    public class dbProvider
    {
        public static Hashtable[] QueryList(string query)
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(query);
                connection.Dispose();
            }
            return datas;
        }
        public static Hashtable QuerySingle(string query)
        {
            Hashtable data;
            using (var connection = CoreSetting.DataConnection)
            {
                data = connection.ExecQueryToHashtable(query);
                connection.Dispose();
            }
            return data;
        }

        public static int ExecuteNonQuery(string query)
        {
            int ret;
            using (var connection = CoreSetting.DataConnection)
            {
                ret = connection.ExecuteWithOpenClose(query);
                connection.Dispose();
            }
            return ret;
        }
    }
}
