using System.Xml.Linq;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP.DIAPI;

namespace ERPService.DataReader
{
    public class QueryString
    {
        public static string DBName
        {
            get { return DIConnection.Instance.CompanyDB; }
        }
        private static string CallStoreBySystem(string query, string param = "")
        {

            return QueryUtil.CallStoreBySystem(DBName, query, param);
        }

        private static string FullQuery(string query)
        {
            
                return string.Format(query);
           
        }

        public static string SAPConnection
        {
            get
            {
                return FullQuery("SELECT * FROM vwSapConnection");
            }
        }

        public static string GetDocumentApprovedQuery
        {
            get
            {
                return CallStoreBySystem("ERPGetDocumentApprove", "F");
            }
        }

        public static string UpdateAfterExecute
        {
            get
            {
                return CallStoreBySystem("ERPAfterExcute", "'{0}',{1} ,{2} , '{3}', '{4}' ");
            }
        }
    }
}
