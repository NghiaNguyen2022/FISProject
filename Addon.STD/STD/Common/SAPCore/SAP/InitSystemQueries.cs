using SAPCore.Config;
using SAPCore.SAP.DIAPI;
using SAPCore.SAP.UserDefine;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SAPCore.SAP
{
    public class InitSystemQueries
    {
        public static string DBName
        {
            get { return DIConnection.Instance.CompanyDB; }
        }

        public static string VersionExistsQuery(string versionId)
        {
            var _tbName = CONSTRANTS.AddonUserDefineVersionTableName;

            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"" + CONSTRANTS.AddonUserDefineVersionTableName + "\"";
            }
            return "SELECT COUNT(1) AS \"CountExist\" " +
                     "FROM " + _tbName +
                    "WHERE \"VersionNumber\" = '" + versionId + "'";
        }
        public static string TablesIsExist(string tableName)
        {
            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                var schema = DBName;
                return "SELECT COUNT(*) AS \"Exist\" " +
                         "FROM TABLES " +
                        "WHERE TABLE_NAME = '" + tableName + "'" +
                          "AND SCHEMA_NAME = '" + DBName + "' ";
            }
            else
            {
                return $@"SELECT COUNT(*) AS Exist
                            FROM sys.TABLES 
                           WHERE name = '{tableName}'";
            }
        }

        public static string CreateTableQuery(string tableName, Dictionary<string, string> columns)
        {
            /*
                CREATE TABLE "AddonUserDefineVersionUDF"
                (
                    "VersionNumber" CHAR(10),
                    "TableName" NVARCHAR(10),
                    "FieldName" NVARCHAR(10),
                    "CreateDate" DATETIME,
                    "Status" CHAR(1),
                    "Message" NVARCHAR(100)
                )*/
            var _tbName = tableName;

            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"" + tableName + "\"";
            }
            var query = " CREATE TABLE " + _tbName +
                        " ( ";
            var index = 0;
            foreach (var item in columns)
            {
                if (index != 0)
                    query += ",";
                query += "\"" + item.Key + "\" " + item.Value;
                index++;
            }
            query += " )";
            return query;
        }

        public static string InitVersionQuery(string versionId)
        {
            var _tbName = CONSTRANTS.AddonUserDefineVersionTableName;

            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"" + CONSTRANTS.AddonUserDefineVersionTableName + "\"";
            }
            return "INSERT INTO " + _tbName +
                     " VALUES ('" + versionId + "', '" + DateTime.Now.ToString("yyyyMMdd") + "', 'Version " + versionId + "') ";
        }

        public static string SaveLogUDFQuery(string versionId, SapUDF field, bool flag, string message)
        {
            var _tbName = CONSTRANTS.AddonUserDefineVersionUDFTableName;

            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"" + CONSTRANTS.AddonUserDefineVersionUDFTableName + "\"";
            }
            return "INSERT INTO " + _tbName + " VALUES ('" + versionId + "', 'F', '" + field.TableName + "', '" + field.FieldName + "', '" + DateTime.Now.ToString("yyyyMMdd") + "', '" + (!flag ? "F" : "S") + "' , '" + message + "')";
        }
        public static string SaveLogDTQuery(string versionId, string tableName, bool flag, string message)
        {
            var _tbName = CONSTRANTS.AddonUserDefineVersionUDFTableName;

            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"" + CONSTRANTS.AddonUserDefineVersionUDFTableName + "\"";
            }
            return "INSERT INTO " + _tbName + " VALUES ('" + versionId + "', 'T', '" + tableName + "', '', '" + DateTime.Now.ToString("yyyyMMdd") + "', '" + (!flag ? "F" : "S") + "' , '" + message + "')";
        }

        public static string GetFieldIDQuery(string tableName, string fieldName)
        {
            var _tbName = "CUFD";
            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                _tbName = "\"" + DBName + "\".\"CUFD\"";
            }
            return "select \"FieldID\" from " + _tbName + " where \"TableID\" = '" + tableName + "' and \"AliasID\" = '" + fieldName + "';";
        }

        public static string GetFieldID
        {
            get
            {
                if (CoreSetting.System == SystemType.SAP_HANA)
                    return "SELECT \"FieldID\" FROM \"CUFD\" WHERE \"TableID\" = '{0}' and \"AliasID\" = '{1}';";
                else
                    return @"SELECT FieldID FROM CUFD WHERE TableID = '{0}' and AliasID = '{1}'";
            }
        }

        public static string CallStoreBySystem(string query, string param = "")
        {
            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                var schema = ConfigurationManager.AppSettings["Schema"];
                return "CALL \"" + schema + "\".\"" + query + "\" (" + param + ")";
            }
            else
            {
                return "EXEC " + query + " " + param;
            }
        }

        public static string CheckTableExists
        {
            get
            {
                return CallStoreBySystem("ExistsTable", "'{0}'");
            }
        }
        public static string QueryCRUD(string query)
        {
            if (CoreSetting.System == SystemType.SAP_HANA)
            {
                var schema = ConfigurationManager.AppSettings["Schema"];
                return "";// CALL \"" + schema + "\".\"" + query + "\" (" + param + ")";
            }
            else
            {
                return "";// "EXEC " + query + " " + param;
            }
        }

    }
}
