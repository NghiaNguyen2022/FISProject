using System;

namespace SAPCore.SAP.DIAPI
{
    public class ActionViaDI
    {
        public static bool ConnectDI(ref string message)
        {
            try
            {
                var retConnect = DIConnection.Instance.Connect(ref message, DIConnection.Instance.CookieConnection);
                if (!retConnect)
                {
                    var mess = DIConnection.Instance.Company.GetLastErrorDescription();
                    message = mess;// STRING_CONTRANTS.CanNotConnectDIAPI + " - " + mess;
                }
                return retConnect;
            }
            catch (Exception ex)
            {
                message = ex.Message;// STRING_CONTRANTS.CanNotConnectDIAPI + " - " + ex.Message;
                return false;
            }
        }
        public static void DisConnectDI()
        {
            DIConnection.Instance.DIDisconnect();
        }
    }
}
