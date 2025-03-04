using APICore.Authen.Atts;
using PN.ApplicationAPI.APICore.Controller;
using PN.ApplicationAPI.Models;
using PN.SmartLib.Helper;
using SAPCore.Config;
using STD.DataReader;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PN.ApplicationAPI.Controllers
{
    public class PaymentController : BaseController
    {
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        //[Route("get1/{param1}")]
        public IHttpActionResult Create([FromBody]PaypooRequest input)
        {
            if (input == null)
            {
                return Ok(ResponseFaild("Data chưa đúng cấu trúc và định dạng"));
            }
            var objJson = new JavaScriptSerializer();
            var objBody = objJson.Deserialize<Payoo_Payment>(input.ResponseData);
            if(objBody == null)
            {
                if (string.IsNullOrEmpty(input.ResponseData))
                {
                    return Ok(ResponseFaild("Data chưa đúng cấu trúc và định dạng"));
                }
            }

            var dbName = ConfigurationManager.AppSettings["Schema"];
            if (objBody.ShopId.ToString() == ConfigurationManager.AppSettings["ShopID"].ToString())
            {
                if(objBody.Orders == null)
                {
                    return Ok(ResponseFaild("Shop ID 6629 cần phải có Orders con"));
                }
                foreach(var order in objBody.Orders)
                {
                    var query = "CALL \"" + dbName + "\".\"usp_Payoo_CheckOrderExists\" ('" + objBody.OrderNo + "', '" + order.OrderNo + "')";
                    var data = dbProvider.QuerySingle(query);
                    if (data != null)
                    {
                        if (data["Existed"].ToString() == "Existed")
                        {
                            return Ok(ResponseFaild("Order No đã tồn tại"));
                        }
                        else
                        {
                            if (data["Existed"].ToString() == "-1")
                            {
                                return Ok(ResponseFaild($"Trên SAP không có invoice mapping với Order No {order.OrderNo } "));
                            }
                        }
                    }
                }
            }
            else
            {
                var query = "CALL \"" + dbName + "\".\"usp_Payoo_CheckOrderExists\" ('" + objBody.OrderNo + "', '" + objBody.OrderNo + "')";
                var data = dbProvider.QuerySingle(query);
                if (data != null)
                {
                    if (data["Existed"].ToString() == "Existed")
                    {
                        return Ok(ResponseFaild("Order No đã tồn tại"));
                    }
                    else
                    {
                        if (data["Existed"].ToString() == "-1")
                        {
                            return Ok(ResponseFaild($"Trên SAP không có invoice mapping với Order No đã tồn tại { objBody.OrderNo}"));
                        }
                    }
                }
            }
            var message = string.Empty;
            if (objBody.InsertData(ref message))
                return Ok(ResponseSuccessed(message));
            return Ok(ResponseFaild(message));
            //return Ok(ResponseSuccessed("OK"));
        }

        [Route("api/Payment/Invoice")]
        //[BasicAuthentication]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Invoice(string customerId, string taxNumber)
        {
            if(string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(taxNumber))
            {
                return Ok(ResponseFaild("Vui lòng tuyền Customer Id và Tax number"));
            }
           
            var dbName = ConfigurationManager.AppSettings["Schema"];
            var query = "CALL \"" + dbName + "\".\"usp_Bank_PayooInvoiceNoPay\" ('" + customerId + "', '" + taxNumber + "')";

            var hashData = dbProvider.QueryList(query);
            var datas = new List<Payoo_InvoiceRes>();

            if (hashData != null && hashData.Length > 0)
            {
                foreach (var item in hashData)
                {
                    var data = new Payoo_InvoiceRes()
                    {
                        invoice_number = item["DocEntry"].ToString(),
                        vat_invoice_serial = item["VatInvoiceNumber"].ToString(),
                        vat_invoice_number = !string.IsNullOrEmpty(item["VatInvoiceNumber"].ToString())
                                             ? item["VatInvoiceNumber"].ToString().Split('.')[1] : string.Empty,
                        invoice_date = item["DocDate"].ToString(),
                        total_amount = decimal.Parse(item["InsTotal"].ToString()),
                        paid_amount = decimal.Parse(item["PaidToDate"].ToString()),
                        remaining_amount = decimal.Parse(item["Remain"].ToString())
                    };
                    datas.Add(data);
                }
            }
            //var data = new Payoo_InvoiceRes()
            //{
            //    invoice_number = "123",
            //    vat_invoice_serial = "CTK/ST",
            //    vat_invoice_number = "AK124",
            //    invoice_date = "20241109",
            //    total_amount = 390000,
            //    paid_amount = 90000,
            //    remaining_amount = 300
            //};

            //var data1 = new Payoo_InvoiceRes()
            //{
            //    invoice_number = "124",
            //    vat_invoice_serial = "CTK/ST",
            //    vat_invoice_number = "AK124",
            //    invoice_date = "20240909",
            //    total_amount = 340000,
            //    paid_amount = 120000,
            //    remaining_amount = 220000
            //};
            //var datas = new List<Payoo_InvoiceRes>();
            //datas.Add(data);
            //datas.Add(data1);
            return Ok(datas);
        }
    }
}