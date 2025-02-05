using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace SAPCore.SAP.UserDefine
{
    public class SapUDT
    {
        public string Version { get; set; }
        public string TableName { get; set; }
        public string TableDesc { get; set; }
        public SAPbobsCOM.BoUTBTableType TableType { get; set; }

        public SapUDT()
        {

        }

        public SapUDT(string version, string tableName, string tableDesc, BoUTBTableType tableType)
        {
            Version = version;
            TableName = tableName;
            TableDesc = tableDesc;
            TableType = tableType;
        }
        public bool Create(ref string message)
        {
            var success = false;
            if (SAPHandler.Instance.TableIsExisted(TableName))
            {
                message = $"{TableName} is created in previous version!";
                return true;
            }
            var ret = "-1";
            ret = SAPHandler.Instance.CreateUDT(TableName, TableDesc, TableType);

            if (string.IsNullOrEmpty(ret))
            {
                message = $"{TableName} created successfully!";
                success = true;
            }

            if (ret == "-1")
            {
                message = $"{TableName} didn't create!";
                success = false;
            }

            return success;
        }
    }
}
