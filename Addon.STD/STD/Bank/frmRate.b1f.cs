using RestSharp;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Helper;
using STDApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace STDApp.Bank
{
    [FormAttribute("STDApp.Bank.frmRate", "Bank/frmRate.b1f")]
    class frmRate : UserFormBase
    {
        public frmRate()
        {
        }
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.btnCall = ((SAPbouiCOM.Button)(this.GetItem("btnCall").Specific));
            this.btnCall.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCall_ClickBefore);
            this.grData = ((SAPbouiCOM.Grid)(this.GetItem("grData").Specific));
            this.OnCustomInitialize();

        }
        private string DateRate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtDate);
            }
        }

        private static frmRate instance;
        public static bool IsFormOpen = false;
        private void InitControl()
        {

        }
        public static void ShowForm()
        {
            try
            {
                if (instance == null)
                {
                    instance = new frmRate();
                }
                instance.InitControl();
                instance.Show();
                IsFormOpen = true;
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private SAPbouiCOM.StaticText lblDate;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.Button btnCall;
        private SAPbouiCOM.Grid grData;

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void btnCall_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {
                if (string.IsNullOrEmpty(DateRate))
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.Validate_DateSelectNull, UIHelper.MsgType.Msgbox, true);
                    this.Freeze(false);
                    return;
                }
                UIHelper.LogMessage($"Bắt đầu vấn gửi yêu lấy tỉ giá đến ngân hàng", UIHelper.MsgType.StatusBar, false);

                var options = new RestClientOptions(APIVietinBankConstrant.APIVTB)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(APIVietinBankConstrant.RateVTB, Method.Post);
                request.AddHeader("x-ibm-client-id", APIVietinBankConstrant.ClientID);// ConfigurationManager.AppSettings["ClientID"]);
                request.AddHeader("x-ibm-client-secret", APIVietinBankConstrant.ClientSecret);// ConfigurationManager.AppSettings["ClientSecret"]);
                request.AddHeader("Content-Type", "application/json");


                var requestData = new RateRequest
                {
                    requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    //providerId = ConfigurationManager.AppSettings["ProviderId"],
                    //merchantId = ConfigurationManager.AppSettings["MerchantId"],
                    merchantId = APIVietinBankConstrant.MerchantId,// ConfigurationManager.AppSettings["MerchantId"],
                    providerId = APIVietinBankConstrant.ProviderId,// ConfigurationManager.AppSettings["ProviderId"],
                    version = "1.0.1",
                    language = "vi",
                    trans_date = DateTime.ParseExact(DateRate, "yyyyMMdd", null).ToString("MM/dd/yyyy"),// "12/14/2024",
                    clientIP = APIVietinBankConstrant.ClientIP,//  ConfigurationManager.AppSettings["ClientIP"],
                    channel = "ERP",
                    signature = ""//,
                                  // transTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                };
                requestData.signature = (
                         requestData.requestId +
                         requestData.providerId +
                         requestData.merchantId +
                         requestData.trans_date +
                         requestData.channel +
                         requestData.version +
                         requestData.clientIP +
                         requestData.language
                     );
                var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

                requestData.signature = FPT.SHA256_RSA2048.Encrypt(requestData.signature, path);
                var json = JsonSerializer.Serialize(requestData);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var result = response.Content; var rps = JsonSerializer.Deserialize<VTResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                if (rps.status.code != "0")
                {
                    UIHelper.LogMessage($"Lỗi {rps.status.message}", UIHelper.MsgType.Msgbox);
                    return;
                }
                var rate = JsonSerializer.Deserialize<RateResponse>(result);
                if (rate != null)
                {
                    if (rate.ForeignExchangeRateInfo == null || rate.ForeignExchangeRateInfo.Count <= 0)
                    {
                        UIHelper.LogMessage($"Không có dữ liệu", UIHelper.MsgType.StatusBar, true);
                        return;
                    }
                    if (this.grData != null && this.grData.DataTable != null)
                    {
                        this.grData.DataTable.Rows.Clear();
                        foreach (var item in rate.ForeignExchangeRateInfo)
                        {

                            this.grData.DataTable.Rows.Add();

                            var index = grData.DataTable.Rows.Count - 1;
                            this.grData.DataTable.SetValue("Date", index, DateTime.ParseExact(DateRate, "yyyyMMdd", null).ToString("dd/MM/yyyy"));
                            this.grData.DataTable.SetValue("Currency", index, item.Currency);
                            this.grData.DataTable.SetValue("MidRate", index, item.Mid_Rate ?? "0");
                            this.grData.DataTable.SetValue("CashRateBig", index, item.Cash_Rate_Big ?? "0");
                            this.grData.DataTable.SetValue("TransferRate", index, item.Transfer_Rate ?? "0");
                            this.grData.DataTable.SetValue("SellRate", index, item.Sell_Rate ?? "0");
                        }
                        this.grData.AutoResizeColumns();
                    }
                }
            }

            catch (Exception ex)
            {

            }
            this.Freeze(false);
        }
    }
}
