using SAPCore.Config;
using STD.DataReader;
using System;
using System.Configuration;

namespace PN.ApplicationAPI.Models
{
    public class BIDV_Accesstoken
    {
        public string Code { get; set; }
        //public string Date { get; set; }
        public bool InsertData(ref string message)
        {
            //using(var trans)
            try
            {
                var dbName = CoreSetting.System == SystemType.SAP_HANA ?
                    ConfigurationManager.AppSettings["Schema"] + "\"." : ConfigurationManager.AppSettings["Schema"] + "\"..";
                var query = "INSERT INTO \"" + dbName + "\"tb_Bank_BIDV_AccesstokenINT\" VALUES ( ";
                query += $"'{Code}',";
                query += $"'{DateTime.Now.ToString("yyyyMMdd")}',";
                query += $"'{DateTime.Now.ToString("HHmmss")}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
                if (ret1 == 1)
                {
                    message = "Lưu thành công";
                }
                else
                {
                    message = "Lưu thất bại";
                }
                return ret1 == 1;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}