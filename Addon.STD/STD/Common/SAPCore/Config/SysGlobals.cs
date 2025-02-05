using SAPCore.SAP.DIAPI;

namespace SAPCore.Config
{
    public class SysGlobals
    {
        public string AddonName { get; set; }
        public string Version { get; set; }
        public int FlagVersion { get; set; }
        public string CusPMFolderID { get; set; }
        public string CusPMFolderDesc { get; set; }
        public string ParentMenuID { get; set; }
        

        public string DBName
        {
            get { return DIConnection.Instance.CompanyDB; }
        }

        public int Language
        {
            get
            {
                if (DIConnection.Instance.Company == null)
                    return -1;
                return (int) DIConnection.Instance.SBO_Application.Language;
            }
        }

        public string UserName
        {
            get
            {
                if (DIConnection.Instance.SBO_Application != null &&
                    DIConnection.Instance.SBO_Application.Company != null)
                    return DIConnection.Instance.SBO_Application.Company.UserName;
                return string.Empty;
            }
        }

        public string LocalCurrencyDefault = "VND";
    }
}
