using SAPCore.Config;
using SAPbobsCOM;
using System;

namespace SAPCore.SAP.DIAPI
{
    /// <summary>
    /// This class to help connect SAP b1 via DI API 
    /// </summary>
    public class DIConnection
    {
        private static DIConnection instance;

        /// <summary>
        /// Singleton pattern to control creation one Instance to connect DI API
        /// </summary>
        public static DIConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = new DIConnection();
                return instance;
            }
        }
        public bool IsConnected = false;


        public string CompanyDB
        {
            get
            {
                if (SBO_Application != null && SBO_Application.Company != null)
                    return SBO_Application.Company.DatabaseName;
                return string.Empty;
            }
        }
        public Company Company { get; set; }
        public SAPbouiCOM.Application SBO_Application;

        public string CookieConnection
        {
            get
            {
                if (string.IsNullOrEmpty(cookieConnection))
                    cookieConnection = Company.GetContextCookie();
                return cookieConnection;
            }
        }
        private string cookieConnection;
        //public string GetCookieConnection()
        //{
        //    if (Company == null)
        //        return string.Empty;
        //    return Company.GetContextCookie();
        //}
        /// <summary>
        /// Connect to DI API 
        /// </summary>
        /// <param name="message">out message to raise when error or exception</param>
        /// <returns></returns>
        private int ConnectDI(ref string message, string connectcookie = "")
        {
            int ret, lErrCode;
            try
            {
                if(Company == null)
                {
                    ret = -1;
                    message = string.Format("Can not init company");
                }
                //Company = new SAPbobsCOM.Company();
                //var sCookie = GetCookieConnection();
                //if(string.IsNullOrEmpty(sCookie))
                //{
                //    sCookie = Company.GetContextCookie();
                //}
                if (string.IsNullOrEmpty(connectcookie))
                    connectcookie = CookieConnection;
                var sConnectionContext = SBO_Application.Company.GetConnectionContext(connectcookie);
                if (Company.Connected == true)
                {
                    Company.Disconnect();
                }
                ret = Company.SetSboLoginContext(sConnectionContext);
                if (ret != 0)
                    Company.GetLastError(out lErrCode, out message);
            }
            catch (Exception ex)
            {
                ret = -1;
                message = string.Format("Error ConnectDI : {0}", ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Connect to Company 
        /// </summary>
        /// <param name="message">out message to raise when error or exception</param>
        /// <returns></returns>
        private int ConnectToCompany(ref string message)
        {
            int ret, lErrCode;
            try
            {
                ret = Company.Connect();
                if (ret != 0)
                    Company.GetLastError(out lErrCode, out message);
            }
            catch (Exception ex)
            {
                ret = -1;
                message = string.Format("Error ConnectToCompany : ", ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// Task execute 2 step, Connect DI and Connect Company
        /// </summary>
        /// <param name="message">out message to raise when error or exception</param>
        /// <returns></returns>
        public bool Connect(ref string message, string connectcookie = "")
        {
            if (ConnectDI(ref message, connectcookie) != 0)
            {
                return false;
            }
            if (ConnectToCompany(ref message) != 0)
            {
                return false;
            }
            return true;
        }
        private DIConnection()
        {
            var message = string.Empty;
            try
            {

                SAPbouiCOM.SboGuiApi SboGuiApi = null;
                string sConnectionString = null;

                SboGuiApi = new SAPbouiCOM.SboGuiApi();
                sConnectionString = CoreSetting.SAPDIConnectstring;

                SboGuiApi.Connect(sConnectionString);
                SBO_Application = SboGuiApi.GetApplication();
                Company = new SAPbobsCOM.Company();

                //if(ConnectDI(ref message) == 0)
                //{

                //}

                //ServerName = ConfigurationManager.AppSettings["SvName"];
                //DBCode = ConfigurationManager.AppSettings["DbName"];
                //DBUserName = ConfigurationManager.AppSettings["DbU"];
                //DBUserPass = StringUtils.DecryptString(ConfigurationManager.AppSettings["DbP"]);
                //UserName = ConfigurationManager.AppSettings["SapU"];
                //UserPass = StringUtils.DecryptString(ConfigurationManager.AppSettings["SapP"]);
                //LicenseServer = ConfigurationManager.AppSettings["SldServer"];
                //SLDServer = "https://" + LicenseServer;
                //var message = string.Empty;
                //SetApplication(ref message);
                //Company = new Company
                //{
                //    // SQL Server
                //    Server = ConfigurationManager.AppSettings["dataSource"],
                //    // SQL Database Name
                //    CompanyDB = DBCode,
                //    UseTrusted = false,
                //    // SAP Account
                //    UserName = UserName,
                //    Password = UserPass,
                //    // SAP License Server
                //    LicenseServer = LicenseServer,
                //    SLDServer = "https://" + LicenseServer,
                //    language = BoSuppLangs.ln_English,
                //    // SQL Account
                //    DbUserName = DBUserName,
                //    DbPassword = DBUserPass// StringUtils.DecryptString(DBUserPass)
                //};
                //switch (CoreSetting.System)
                //{
                //    case SystemType.SAP_HANA:
                //        Company.DbServerType = BoDataServerTypes.dst_HANADB; break;
                //    case SystemType.SAP_SQL_2014:
                //        Company.DbServerType = BoDataServerTypes.dst_MSSQL2014; break;
                //    case SystemType.SAP_SQL_2016:
                //        Company.DbServerType = BoDataServerTypes.dst_MSSQL2016; break;
                //    case SystemType.SAP_SQL_2017:
                //        Company.DbServerType = BoDataServerTypes.dst_MSSQL2017; break;
                //    default:
                //        Company.DbServerType = BoDataServerTypes.dst_MSSQL2019; break;
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public bool DIDisconnect()
        {
            try
            {
                if (Company.Connected)
                    Company.Disconnect();
            }
            catch (Exception ex)
            {
                // message = string.Format("Disconnect fail - {0}", ex.Message);
                return false;
            }
            return true;
        }

        public int GetFieldID(string tableName, string fieldName)
        {
            int index = -1;
            try
            {

            }
            catch (Exception ex)
            {

            }
            return index;
        }
    }
}
