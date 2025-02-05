using PN.SmartLib.Connection;
using PN.SmartLib.Helper;
using SAPCore.SAP.UserDefine;
using System;
using System.Collections.Generic;

namespace SAPCore.SAP
{
    public abstract class VerBuilder
    {
        public delegate void MessageVerBuilderHandler(string message);
        public static int Version = 1;

        protected int InternalVersion;
        protected List<SapUDF> ListField;
        protected List<SapUDT> ListTable;
        public event MessageVerBuilderHandler OnMessage;

        public abstract int Init();
        // public abstract int RunCreate(ref string message);
        public int RunCreate(BaseConnection connection, ref string message)
        {
            if (!VersionExist(connection))
            {
                OnMessage?.Invoke("Start create UDFs.");
                var ret = Create(connection, ref message);
                if (ret == 1)
                {
                    InitVersion(connection);
                }

                OnMessage?.Invoke("End create UDFs.");
            }
            message = "Version này tồn tại";
            return 0;
        }

        protected int Create(BaseConnection connection, ref string message)
        {
            try
            {
                if (ListTable != null)
                {
                    foreach (var item in ListTable)
                    {
                        message = string.Empty;
                        if (item.TableName.Contains("V_MONTHLYCONSUM"))
                        {

                        }
                        OnMessage?.Invoke($"Start create table {item.TableName}");
                        CreateUDT(connection, item, ref message);
                        OnMessage?.Invoke(message);
                    }
                }

                if (ListField != null)
                {
                    foreach (var item in ListField)
                    {
                        message = string.Empty;
                        OnMessage?.Invoke($"Start create field {item.FieldName} in {item.TableName}");
                        CreateUDF(connection, item, ref message);
                        OnMessage?.Invoke(message);
                    }
                }
            }
            catch (Exception)
            {
                message = "Cai gi do";
                return -1;
            }
            return 1;
        }
        protected void CreateUDT(BaseConnection connection, SapUDT table, ref string message)
        {
            //SaveUDTLog(connection, table.TableName, ret1, message);
            try
            {
                if(table.TableName.Contains("V_LABORCOSTDETAIL"))
                {

                }
                var ret1 = table.Create(ref message);
                var query = InitSystemQueries.SaveLogDTQuery(InternalVersion.ToString(), table.TableName, ret1, message);
                DataProvider.ExecuteNonQuery(connection, query);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        protected void CreateUDF(BaseConnection connection, SapUDF field, ref string message)
        {
            //var message = string.Empty;
           // SaveUDFLog(connection, field, ret1, message);
            try
            {
                var ret1 = field.Create(ref message);
                var query = InitSystemQueries.SaveLogUDFQuery(InternalVersion.ToString(), field, ret1, message
                    );
                DataProvider.ExecuteNonQuery(connection, query);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        protected bool VersionExist(BaseConnection connection)
        {
            var exsit = false;
            try
            {
                // check version table is exist
                var querytbHeader = InitSystemQueries.TablesIsExist(CONSTRANTS.AddonUserDefineVersionTableName);
                var querytbDetail = InitSystemQueries.TablesIsExist(CONSTRANTS.AddonUserDefineVersionUDFTableName);

                var datatbHeader = DataProvider.QuerySingle(connection, querytbHeader);
                var datatbDt = DataProvider.QuerySingle(connection, querytbDetail);
                if (datatbHeader == null || datatbDt == null)
                {
                    return false;
                }

                if (datatbHeader["Exist"].ToString() == "0")
                {
                    var columns = new Dictionary<string, string>();
                    columns.Add("VersionNumber", "CHAR(10)");
                    columns.Add("CreateDate", "DATETIME");
                    columns.Add("Note", "NVARCHAR(100)");

                    var queryCreatetb = InitSystemQueries.CreateTableQuery(CONSTRANTS.AddonUserDefineVersionTableName, columns);
                    DataProvider.ExecuteNonQuery(connection, queryCreatetb);
                }
                if (datatbDt["Exist"].ToString() == "0")
                {
                    var columns = new Dictionary<string, string>();
                    columns.Add("VersionNumber", "CHAR(10)");
                    columns.Add("Type", "NVARCHAR(10)");
                    columns.Add("TableName", "NVARCHAR(20)");
                    columns.Add("FieldName", "NVARCHAR(20)");
                    columns.Add("CreateDate", "DATETIME");
                    columns.Add("Status", "CHAR(1)");
                    columns.Add("Message", "NVARCHAR(100)");

                    var queryCreatetb = InitSystemQueries.CreateTableQuery(CONSTRANTS.AddonUserDefineVersionUDFTableName, columns);
                    DataProvider.ExecuteNonQuery(connection, queryCreatetb);
                }

                var query = InitSystemQueries.VersionExistsQuery(InternalVersion.ToString());
                var data = DataProvider.QuerySingle(connection, query);
                if (data != null)
                {
                    if (data["CountExist"].ToString() == "0")
                    {
                        exsit = false;
                    }
                    else
                    {
                        exsit = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return exsit;
        }

        protected void InitVersion(BaseConnection connection)
        {
            try
            {
                var query = InitSystemQueries.InitVersionQuery(InternalVersion.ToString());
                DataProvider.ExecuteNonQuery(connection, query);
            }
            catch (Exception ex)
            {

            }
        }

        protected void SaveUDFLog(BaseConnection connection, SapUDF field, bool flag, string message)
        {
            try
            {
                var query = InitSystemQueries.SaveLogUDFQuery(InternalVersion.ToString(), field, flag, message
                    );// $@"INSERT INTO AddonUserDefineVersionUDF
                      // VALUES ('{InternalVersion}',        
                      //                '{field.TableName}',      
                      //              '{field.FieldName}',
                      //            '{DateTime.Now.ToString("yyyyMMdd")}',    
                      //        '{(!flag ? "F" : "S")}',    
                      //         '{message}')";

                DataProvider.ExecuteNonQuery(connection, query);
            }
            catch (Exception ex)
            {

            }
        }
        protected void SaveUDTLog(BaseConnection connection, string tableName, bool flag, string message)
        {
            try
            {
                var query = InitSystemQueries.SaveLogDTQuery(InternalVersion.ToString(), tableName, flag, message);
                DataProvider.ExecuteNonQuery(connection, query);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
