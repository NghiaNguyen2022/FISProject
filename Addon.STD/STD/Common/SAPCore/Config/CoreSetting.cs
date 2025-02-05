using PN.SmartLib.Connection;
using PN.SmartLib.Helper;
using SAPCore.Connection;
using System.Configuration;

namespace SAPCore.Config
{
    /// <summary>
    /// Core SAP b1 Seting for working addon/DI API....
    /// </summary>
    public class CoreSetting
    {
        public static string SAPDIConnectstring = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

        public static string Schema = ConfigurationManager.AppSettings["Schema"];

        public static int UF_HorMargin = 10;
        public static int UF_VerMargin = 30;
        public static int UF_HorizontallySpaced = 20;
        public static int UF_VerticallySpaced = 20;

        // Distance with other control in a group control
        public static int UIInternalHDistance = 7;

        // Distance with other control (no a same group)
        public static int UIExternalHDistance = 10;

        // Distance with other control in a group control
        public static int UIInternalWDistance = 7;

        // Distance with other control (no a same group)
        public static int UIExternalWDistance = 7;

        // Height of system button
        public static int UISystemButtonHeight = 19;
        /// <summary>
        /// Get System Type from config to connect
        /// SAP_HANA or SAP_SQL_2019, SAP_SQL_2017....
        /// </summary>
        public static SystemType System
        {
            get
            {
                var type = ConfigurationManager.AppSettings["SystemType"].ToString().GetEnumValueByDescription<SystemType>();
                return type;// ConfigurationManager.AppSettings["SystemType"].ToString().GetEnumValueByDescription<SystemType>();
            }
        }
        
        /// <summary>
        /// Get connection to Database with ADO.Net
        /// This dependency with SystemType
        /// </summary>
        public static BaseConnection DataConnection
        {
            get
            {
                if (CoreSetting.System == SystemType.SAP_HANA)
                    return new HanaDBConnection(SAPHanaConnectionString);

                return new SQLDBConnection(SAPSQLConnectionString);
            }
        }

        /// <summary>
        /// Get connection string to connect to SQL server using ADO.Net
        /// Get connection string with config app, this config contain info to connect database, and password is hash
        /// </summary>
        public static string SAPSQLConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["SAPConnection"].ConnectionString;
                var pass = StringUtils.DecryptString(ConfigurationManager.AppSettings["HASHSql"]);

                return string.Format(connectionString, pass);
            }
        }
        /// <summary>
        /// Get connection string to connect to Hana server using ADO.Net
        /// Get connection string with config app, this config contain info to connect database, and password is hash
        /// </summary>
        public static string SAPHanaConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["SAPHanaConnection"].ConnectionString;
                var pass = StringUtils.DecryptString(ConfigurationManager.AppSettings["HASHHana"]);

                return string.Format(connectionString, pass);
            }
        }

    }
}
