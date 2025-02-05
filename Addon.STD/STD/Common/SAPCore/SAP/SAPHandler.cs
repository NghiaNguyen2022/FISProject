using PN.SmartLib.Helper;
using SAPCore.Config;
using System;
using System.Collections.Generic;

namespace SAPCore.SAP
{
    public class SAPHandler
    {

        private static SAPHandler instance;
        public static SAPHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SAPHandler();
                }

                return instance;
            }
        }
        public SAPbouiCOM.Application SBO_Application;
        private SAPHandler()
        {
            ConnectSAP.SetApplication();
            //SetApplication();
            //oCompany = new Company();
        }

        public string CreateUDF(string tableName, string fieldName, string desc, SAPbobsCOM.BoFieldTypes fieldType, int Size, Dictionary<string, string> validValues, 
            string LinkTab = "", string TableType = "", string DefaultValue = "", SAPbobsCOM.BoFldSubTypes fieldsubType = SAPbobsCOM.BoFldSubTypes.st_None)
        {
            string ret = "";
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = default(SAPbobsCOM.UserFieldsMD);
            try
            {
                int lRetCode = 0;
                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                oUserFieldsMD.TableName = tableName;
                oUserFieldsMD.Name = fieldName;
                oUserFieldsMD.Description = desc;
                oUserFieldsMD.Type = fieldType;

                foreach (var element in validValues)
                {
                    oUserFieldsMD.ValidValues.Value = element.Key;
                    oUserFieldsMD.ValidValues.Description = element.Value;
                    oUserFieldsMD.ValidValues.Add();
                }

                if (Size > 0)
                {
                    oUserFieldsMD.EditSize = Size;
                }
                

                oUserFieldsMD.SubType = fieldsubType;
                oUserFieldsMD.DefaultValue = DefaultValue;

                if (LinkTab != "")
                {
                    if (TableType == "UDT")
                    {
                        oUserFieldsMD.LinkedTable = LinkTab;
                    }
                    if (TableType == "UDO")
                    {
                        oUserFieldsMD.LinkedUDO = LinkTab;
                    }
                    if (TableType == "OITT")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulProductTrees;
                    }
                    if (TableType == "OITM")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulItems;
                    }
                    if (TableType == "OWHS")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulWarehouses;
                    }
                    if (TableType == "OCRD")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulBusinessPartners;
                    }
                    //if (LinkTab.Substring(0) == "@")
                    //{

                    //}
                    //if (LinkTab== "ORSC")
                    //{
                    //    oUserFieldsMD.LinkedSystemObject= SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.
                    //}
                    //oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum
                }
                lRetCode = oUserFieldsMD.Add();
                if (lRetCode != 0)
                {
                    if ((lRetCode == -2035 | lRetCode == -1120))
                    {
                        ret = Convert.ToString(lRetCode);
                    }
                    return Setting.oCompany.GetLastErrorDescription();
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
        public string CreateUDF(string tableName, string fieldName, string desc, SAPbobsCOM.BoFieldTypes fieldType, int Size, string LinkTab = "", string TableType = "",
            string DefaultValue = "", SAPbobsCOM.BoFldSubTypes fieldsubType = SAPbobsCOM.BoFldSubTypes.st_None)
        {
            var ret = "";
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = default(SAPbobsCOM.UserFieldsMD);
            try
            {
                int lRetCode = 0;
                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
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
                if (LinkTab != "")
                {
                    if (TableType == "UDT")
                    {
                        oUserFieldsMD.LinkedTable = LinkTab;
                    }
                    if (TableType == "UDO")
                    {
                        oUserFieldsMD.LinkedUDO = LinkTab;
                    }
                    if (TableType == "OITT")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulProductTrees;
                    }
                    if (TableType == "OITM")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulItems;
                    }
                    if (TableType == "OWHS")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulWarehouses;
                    }
                    if (TableType == "OCRD")
                    {
                        oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.ulBusinessPartners;
                    }
                    //if (LinkTab.Substring(0) == "@")
                    //{

                    //}
                    //if (LinkTab== "ORSC")
                    //{
                    //    oUserFieldsMD.LinkedSystemObject= SAPbobsCOM.UDFLinkedSystemObjectTypesEnum.
                    //}
                    //oUserFieldsMD.LinkedSystemObject = SAPbobsCOM.UDFLinkedSystemObjectTypesEnum
                }
                lRetCode = oUserFieldsMD.Add();
                if (lRetCode != 0)
                {
                    if ((lRetCode == -2035 | lRetCode == -1120))
                    {
                        ret = Convert.ToString(lRetCode) + " - " + Setting.oCompany.GetLastErrorDescription();
                    }
                    return Setting.oCompany.GetLastErrorDescription();
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
        
        public string CreateUDT(string tableName, string tableDesc, SAPbobsCOM.BoUTBTableType tableType)
        {
            string ret = "";
            SAPbobsCOM.UserTablesMD oUdtMD = null;
            try
            {
                int lRetCode = 0;
                oUdtMD = (SAPbobsCOM.UserTablesMD)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (oUdtMD.GetByKey(tableName) == false)
                {
                    oUdtMD.TableName = tableName;
                    oUdtMD.TableDescription = tableDesc;
                    oUdtMD.TableType = tableType;
                    lRetCode = oUdtMD.Add();
                    if ((lRetCode != 0))
                    {
                        if ((lRetCode == -2035))
                        {
                            ret = "-2035";
                        }
                        ret = Setting.oCompany.GetLastErrorDescription();

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

        public int GetFieldIDByName(string tableName, string fieldName)
        {
            int index = -1;
            //SAPbobsCOM.Recordset ors = default(SAPbobsCOM.Recordset);
            try
            {
                var query = InitSystemQueries.GetFieldIDQuery(tableName, fieldName);
                var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
                if(data != null)
                {
                    CustomConverter.ConvertStringToInt(data["FieldID"].ToString(), ref index);
                }
                //ors = (SAPbobsCOM.Recordset)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                //if (Setting.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                //{
                //    ors.DoQuery("select \"FieldID\" from \"CUFD\" where \"TableID\" = '" + tableName + "' and \"AliasID\" = '" + fieldName + "';");
                //}
                //else
                //{
                //    ors.DoQuery("select FieldID from CUFD where TableID = '" + tableName + "' and AliasID = '" + fieldName + "'");
                //}

                //if (!ors.EoF)
                //{
                //    index = (int)ors.Fields.Item("FieldID").Value;
                //}
            }
            catch (Exception ex)
            {

            }

            return index;
        }
        public bool FieldIsExisted(string tableName, string fieldName, bool isUDT = false)
        {
            bool ret = false;
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = null;
            try
            {
                fieldName = fieldName.Replace("U_", "");

                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                var tbName = tableName;
                if (isUDT)
                    tbName = $"@{tableName}";
                int FieldID = GetFieldIDByName(tbName, fieldName);
                if (oUserFieldsMD.GetByKey(tbName, FieldID))
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
        public bool TableIsExisted(string tableName)
        {
            bool ret = false;
            SAPbobsCOM.UserTablesMD oUdtMD = null;
            try
            {
                //tableName = tableName.Replace("@", "");
                oUdtMD = (SAPbobsCOM.UserTablesMD)Setting.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (oUdtMD.GetByKey(tableName))
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
    }
}
