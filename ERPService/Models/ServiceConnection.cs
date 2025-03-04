using SAPCore.SAP.Model;
using System.Collections;

namespace ERPService.Models
{
    public class ServiceConnection  : SAPConnection
    {

        public ServiceConnection()
        {

        }

        public ServiceConnection(Hashtable data)
        {
            SapUser = data["SapUser"].ToString();
            SapPassword = data["SapPassword"].ToString();
            ServerName = data["ServerName"].ToString();
            CompanyDB = data["CompanyDB"].ToString();
            SLDServer = data["SLDServer"].ToString();
            DBUser = data["DBUser"].ToString();
            DBPass = data["DBPass"].ToString();
        }
    }
}
