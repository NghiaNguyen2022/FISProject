using System.Collections.Generic;
using SAPbobsCOM;

namespace SAPCore.SAP.UserDefine
{
    public class SapUDF
    {
        public string Version { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldDesc { get; set; }

        public SAPbobsCOM.BoFieldTypes FieldType { get; set; }
        public SAPbobsCOM.BoFldSubTypes SubType { get; set; }
        public int Size { get; set; }
        public string LinkTable { get; set; }
        public string TableType { get; set; }
        public string DefValue { get; set; }
        public bool IsUDT { get; set; }

        public string DBFieldName
        {
            get
            {
                return string.Format("U_{0}", FieldName);
            }
        }

        public Dictionary<string, string> ValidValues { get; set; }

        //public void CheckVersion

        public SapUDF()
        {

        }

        public SapUDF(string version, string tableName, string fieldName, string fieldDesc, 
            BoFieldTypes fieldType, SAPbobsCOM.BoFldSubTypes subtype, int size, Dictionary<string, string> validValues, 
            string defValue = "", string link = "", string tableType = "", bool isUDT = false)
        {
            Version = version;
            TableName = tableName;
            FieldName = fieldName;
            FieldDesc = fieldDesc;
            FieldType = fieldType;
            Size = size;
            LinkTable = link;
            DefValue = defValue;
            ValidValues = validValues;
            TableType = tableType;
            LinkTable = link;
            SubType = subtype;
            IsUDT = isUDT;
        }
        public SapUDF(string version, string tableName, string fieldName, string fieldDesc, 
            BoFieldTypes fieldType, SAPbobsCOM.BoFldSubTypes subtype, int size, 
            string defValue = "", bool isUDT = false)
        {
            Version = version;
            TableName = tableName;
            FieldName = fieldName;
            FieldDesc = fieldDesc;
            FieldType = fieldType;
            TableType = string.Empty;
            Size = size;
            LinkTable = string.Empty;
            DefValue = defValue;
            SubType = subtype;
            IsUDT = isUDT;
        }
        public SapUDF(string version, string tableName, string fieldName, string fieldDesc, 
            BoFieldTypes fieldType, SAPbobsCOM.BoFldSubTypes subtype, int size, 
            string link, string tableType, string defValue = "", bool isUDT = false)
        {
            Version = version;
            TableName = tableName;
            FieldName = fieldName;
            FieldDesc = fieldDesc;
            Size = size;
            LinkTable = link;
            FieldType = fieldType;
            TableType = tableType;
            DefValue = defValue;
            SubType = subtype;
            IsUDT = isUDT;
        }

        public bool Create(ref string message)
        {
            var success = false;
            //if(TableName.Contains("TLCD"))
            //{

            //}
            if(SAPHandler.Instance.FieldIsExisted(TableName, DBFieldName, IsUDT))
            {
                message = $"Field {DBFieldName} in {TableName} is created in previous version!";
                return true;
            }
            var ret = "-1";

            if(ValidValues == null || ValidValues.Count == 0)
            {
                ret = SAPHandler.Instance.CreateUDF(TableName, FieldName, FieldDesc, FieldType, Size, LinkTable, TableType, DefValue, SubType);
            }
            else
            {
                ret = SAPHandler.Instance.CreateUDF(TableName, FieldName, FieldDesc, FieldType, Size, ValidValues, LinkTable, TableType, DefValue, SubType);
            }

            if(string.IsNullOrEmpty(ret))
            {
                message = $"Field {DBFieldName} in {TableName} created successfully!";
                success = true;
            }

            if (ret == "-1")
            {
                message = $"Field {DBFieldName} in {TableName} didn't create!";
                success =  false;
            }
            
            return success;
        }
    }
}
