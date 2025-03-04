using APICore.Authen.Atts;
using PN.ApplicationAPI.APICore.Controller;
using PN.ApplicationAPI.Models;
using SAPCore.Config;
using STD.DataReader;
using System;
using System.Configuration;
using System.Web.Http;

namespace PN.ApplicationAPI.Controllers
{
    public class BankController : BaseController
    {
        //[BasicAuthentication]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Create(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(ResponseFaild("Data chưa đúng cấu trúc và định dạng"));
            }

            var message = string.Empty;
            if (InsertData(code, ref message))
                return Ok(ResponseSuccessed(message));
            return Ok(ResponseFaild(message));
        }

        private bool InsertData(string code, ref string message)
        {
            //using(var trans)
            try
            {
                var dbName = ConfigurationManager.AppSettings["Schema"];
                var query = "INSERT INTO \"" + dbName + "\".\"tb_Bank_BIDV_AccesstokenINT\" VALUES ( ";
                query += $"'{code}',";
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
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}