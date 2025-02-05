using STD.DataReader;
using PN.SmartLib.Helper;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP.DIAPI;
using System;
using System.Collections;

namespace STD.Utils
{
    public class ConfigHelper
    {
        public static int GetAddonVersion(string name, string version)
        {
            var query = "SELECT COUNT(\"Name\") AS \"CountVersion\" " + 
                          "FROM \"" + CoreSetting.Schema + "\".\"@V_VERSION\" " + 
                         "WHERE \"Code\" = '" + name + "' AND \"Name\" = '" + version +"'";
            Hashtable data;
            using (var connection = CoreSetting.DataConnection)
            {
                data = connection.ExecQueryToHashtable(query);
                connection.Dispose();
            }
            if(data != null)
            {
                var countText = data["CountVersion"].ToString();
                var count = 0;
                if(int.TryParse(countText, out count))
                {
                    return count;
                }
            }
            return 0;
        }

        public static DateTime LastDateOfMonth(string period, string year)
        {
            var query = "SELECT \"T_RefDate\" FROM \"" + DIConnection.Instance.CompanyDB + "\".OFPR WHERE \"Category\" = '" + year + "' AND \"Code\" = '" + period + "' ";
            var date = DateTime.Now;
            var data = dbProvider.QuerySingle(query);
            if (data != null && !string.IsNullOrEmpty(data["T_RefDate"].ToString()))
            {
                CustomConverter.ConvertStringToDate(data["T_RefDate"].ToString(), ref date);
            }

            return date;
        }
    }
}
