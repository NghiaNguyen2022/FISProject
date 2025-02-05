using SAPbobsCOM;
using System;

namespace SAPCore.SAP
{
    public class Setting
    {
        public static SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
    }
    public class ConnectSAP
    {
        public static SAPbobsCOM.Company oCompany;
        public /* TRANSINFO: WithEvents */ static SAPbouiCOM.Application SBO_Application;
        public static void SetApplication()
        {
            try
            {
                SAPbouiCOM.SboGuiApi SboGuiApi = null;
                string sConnectionString = null;

                SboGuiApi = new SAPbouiCOM.SboGuiApi();

                // // by following the steps specified above, the following
                // // statment should be suficient for either development or run mode
                //try
                //{
                //    sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));
                //}
                //catch //(Exception ex)
                //{
                //    sConnectionString = "";
                //}

                //if (sConnectionString == "")
                //{
                //    sConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
                //}
                sConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

                // // connect to a running SBO Application

                SboGuiApi.Connect(sConnectionString);

                // // get an initialized application object

                SBO_Application = SboGuiApi.GetApplication(-1);
                SetApplicationDI();
                Setting.oCompany = new SAPbobsCOM.Company();
                Setting.oCompany = ConnectSAP.oCompany;
                //Message.MsgBoxWrapper("Connected SAP B1 Of VATIntegration");
            }
            catch (Exception ex)
            {
                //Message.MsgBoxWrapper("Error SetApplication VATIntegration: " + ex.Message, "", true);
            }


        }
        public static void SetApplicationDI()
        {
            if (!(ConnectDI() == 0))
            {
                SBO_Application.MessageBox("Failed setting a connection to DI API : ConectDI " + ConnectDI().ToString());
                return;
            }
            if (!(ConnectToCompany() == 0))
            {
                SBO_Application.MessageBox("Failed connecting to the company's Data Base : ConnectToCompany " + ConnectToCompany().ToString());
                return;
            }
            ////Message.MsgBoxWrapper("Connected SAP B1 Of VATIntegration");
        }
        private static int ConnectDI()
        {
            int ConnectDIRetun;
            try
            {
                String sCookie = null;
                String sConnectionContext = null;
                oCompany = new Company();
                sCookie = oCompany.GetContextCookie();
                sConnectionContext = SBO_Application.Company.GetConnectionContext(sCookie);
                if (oCompany.Connected == true)
                {
                    oCompany.Disconnect();
                }
                ConnectDIRetun = oCompany.SetSboLoginContext(sConnectionContext);
            }
            catch (Exception ex)
            {
                ConnectDIRetun = -1;
                //Message.MsgBoxWrapper("Error ConnectDI : " + ex.Message, "", true);
            }

            return ConnectDIRetun;
        }
        private static int ConnectToCompany()
        {
            int ConnectToCompanyReturn;
            try
            {
                ConnectToCompanyReturn = oCompany.Connect();
            }
            catch (Exception ex)
            {
                ConnectToCompanyReturn = -1;
                //Message.MsgBoxWrapper("Error ConnectToCompany : " + ex.Message, "", true);

            }


            return ConnectToCompanyReturn;
        }
        public static string GetInvoiceKey(SAPbobsCOM.Documents trx)
        {
            string keytype = "";
            string key = "";
            string TransUid = "";
            try
            {
                try
                {
                    TransUid = trx.UserFields.Fields.Item("U_TransUid").Value.ToString();
                }
                catch (Exception ex)
                {
                    TransUid = "";
                }
                if (string.IsNullOrEmpty(TransUid))
                {
                    if (trx.DocObjectCode == SAPbobsCOM.BoObjectTypes.oInvoices)
                    {
                        keytype = "IN";
                    }
                    if (trx.DocObjectCode == SAPbobsCOM.BoObjectTypes.oCreditNotes)
                    {
                        keytype = "CN";
                    }
                    if (trx.DocObjectCode == SAPbobsCOM.BoObjectTypes.oDeliveryNotes)
                    {
                        keytype = "DN";
                    }
                    if (trx.DocObjectCode == SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes)
                    {
                        keytype = "PC";
                    }
                    key = keytype + trx.DocDate.ToString("yyyyMMdd") + trx.DocEntry.ToString();
                }
                else
                {
                    key = TransUid;
                }
            }
            catch (Exception ex)
            {

            }

            return key;
        }
        public int GetFieldidByName(string TableName, string FieldName)
        {
            int index = -1;
            SAPbobsCOM.Recordset ors = default(SAPbobsCOM.Recordset);
            try
            {
                ors = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                if (oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    ors.DoQuery("select \"FieldID\" from \"CUFD\" where \"TableID\" = '" + TableName + "' and \"AliasID\" = '" + FieldName + "';");
                }
                else
                {
                    ors.DoQuery("select FieldID from CUFD where TableID = '" + TableName + "' and AliasID = '" + FieldName + "'");
                }

                if (!ors.EoF)
                {
                    index = (int)ors.Fields.Item("FieldID").Value;
                }
            }
            catch (Exception ex)
            {

            }

            return index;
        }
        public bool CheckFieldExists(string TableName, string FieldName)
        {
            bool ret = false;
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = null;
            try
            {
                FieldName = FieldName.Replace("U_", "");
                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);

                int FieldID = GetFieldidByName(TableName, FieldName);
                if (oUserFieldsMD.GetByKey(TableName, FieldID))
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }

            }
            catch (Exception ex)
            {
                ret = false;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                oUserFieldsMD = null;
                GC.Collect();
            }
            return ret;
        }
        public bool CheckTableExists(string TableName)
        {
            bool ret = false;
            SAPbobsCOM.UserTablesMD oUdtMD = null;
            try
            {
                TableName = TableName.Replace("@", "");
                oUdtMD = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (oUdtMD.GetByKey(TableName))
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUdtMD);
                oUdtMD = null;
                GC.Collect();
            }
            return ret;
        }

        public string CreateUDT(string tableName, string tableDesc, SAPbobsCOM.BoUTBTableType tableType)
        {
            string ret = "";
            SAPbobsCOM.UserTablesMD oUdtMD = null;
            try
            {
                int lRetCode = 0;
                oUdtMD = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (oUdtMD.GetByKey(tableName) == false)
                {
                    oUdtMD.TableName = tableName;
                    oUdtMD.TableDescription = tableDesc;
                    oUdtMD.TableType = tableType;
                    //Message.MsgBoxWrapper(String.Format("Adding UDT {0}", tableName), "", false);
                    lRetCode = oUdtMD.Add();
                    if ((lRetCode != 0))
                    {
                        if ((lRetCode == -2035))
                        {
                            ret = "-2035";
                        }
                        ret = oCompany.GetLastErrorDescription();
                        //Message.MsgBoxWrapper(String.Format("UDT {0} Error {1}", tableName, ret), "", true);
                    }
                    else
                    {
                        //Message.MsgBoxWrapper(String.Format("Operation completed successfully UDT {0}", tableName), "", false);
                    }

                    ret = "";
                }
                else
                {
                    ret = "";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUdtMD);
                oUdtMD = null;
                GC.Collect();
            }
            return ret;

        }
        public string CreateUDF(string tableName, string fieldName, string desc, SAPbobsCOM.BoFieldTypes fieldType, SAPbobsCOM.BoFldSubTypes fieldsubType, int Size, string LinkTab = "",
            string DefaultValue = "", string companyDB = "")
        {
            string ret = "";
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = default(SAPbobsCOM.UserFieldsMD);
            try
            {
                int lRetCode = 0;
                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                oUserFieldsMD.TableName = tableName;
                oUserFieldsMD.Name = fieldName;
                oUserFieldsMD.Description = desc;
                oUserFieldsMD.Type = fieldType;
                if (Size > 0)
                {
                    oUserFieldsMD.EditSize = Size;
                }

                oUserFieldsMD.SubType = fieldsubType;
                oUserFieldsMD.DefaultValue = DefaultValue;
                //Message.MsgBoxWrapper(String.Format("Adding UDT {0} UDF {1}", tableName, fieldName), "", false);
                lRetCode = oUserFieldsMD.Add();
                if (lRetCode != 0)
                {
                    if ((lRetCode == -2035 | lRetCode == -1120))
                    {
                        ret = Convert.ToString(lRetCode);
                    }

                    //Message.MsgBoxWrapper(String.Format("UDT {0} UDF {1} Error {2}", tableName, fieldName, oCompany.GetLastErrorDescription()), "", true);
                    return oCompany.GetLastErrorDescription();
                }
                else
                {
                    //Message.MsgBoxWrapper(String.Format("Operation completed successfully UDT {0} UDF {1}", tableName, fieldName), "", false);
                }

                ret = "";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            finally
            {
            }
            return ret;
        }
    }
    
}
