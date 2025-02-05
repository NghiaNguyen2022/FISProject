using SAPCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPCore.Helper
{
    public class QueryUtil
    {
        public static string CallStoreBySystem(string dbname, string query, string param = "")
        {
            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                //var schema = Globals.DBName; //ConfigurationManager.AppSettings["Schema"];
                //var schema1 = Globals.DBName;
                return "CALL \"" + dbname + "\".\"" + query + "\" (" + param + ")";
            }
            else
            {
                return "EXEC " + query + " " + param;
            }
        }
    }
}
